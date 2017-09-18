using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NetworkUI;
using Nac.Wpf.SectionEditor.NetworkModel;
using Nac.Wpf.Common.Control;
using Nac.Common;
using Nac.Wpf.Common;
using System.Windows.Data;
using System;
using System.Globalization;
using Nac.Common.Control;
using System.Windows.Media;
using System.Collections.Generic;

namespace Nac.Wpf.SectionEditor {
    public partial class NacWpfSectionEditor1 : UserControl {

        public NacWpfSectionEditor1() { InitializeComponent(); }

        private NacWpfSection Section { get { return (DataContext as NacWpfSectionViewModel1).Section; } }

        private NacWpfEngine Engine { get { return Section.Engine; } }
        private string Path { get { return Section.Path; } }
        private IEnumerable<NacWpfBlock> Blocks { get { return Section.Blocks; } }

        public NacWpfSectionViewModel1 ViewModel { get { return (NacWpfSectionViewModel1)DataContext; } }

        #region connection dragging
        private void networkControl_ConnectionDragStarted(object sender, ConnectionDragStartedEventArgs e) {
            var draggedOutConnector = (NacWpfConnectorViewModel1)e.ConnectorDraggedOut;
            var curDragPoint = Mouse.GetPosition(networkControl);
            var connection = this.ViewModel.ConnectionDragStarted(draggedOutConnector, curDragPoint);
            e.Connection = connection;
        }

        private void networkControl_QueryConnectionFeedback(object sender, QueryConnectionFeedbackEventArgs e) {
            var draggedOutConnector = (NacWpfConnectorViewModel1)e.ConnectorDraggedOut;
            var draggedOverConnector = (NacWpfConnectorViewModel1)e.DraggedOverConnector;
            object feedbackIndicator = null;
            bool connectionOk = true;

            ViewModel.QueryConnnectionFeedback(draggedOutConnector, draggedOverConnector, out feedbackIndicator, out connectionOk);
            e.FeedbackIndicator = feedbackIndicator;
            e.ConnectionOk = connectionOk;
        }

        private void networkControl_ConnectionDragging(object sender, ConnectionDraggingEventArgs e) {
            Point curDragPoint = Mouse.GetPosition(networkControl);
            var connection = (NacWpfConnectionViewModel1)e.Connection;
            ViewModel.ConnectionDragging(curDragPoint, connection);
        }

        private void networkControl_ConnectionDragCompleted(object sender, ConnectionDragCompletedEventArgs e) {
            var connectorDraggedOut = (NacWpfConnectorViewModel1)e.ConnectorDraggedOut;
            var connectorDraggedOver = (NacWpfConnectorViewModel1)e.ConnectorDraggedOver;
            var newConnection = (NacWpfConnectionViewModel1)e.Connection;
            ViewModel.ConnectionDragCompleted(newConnection, connectorDraggedOut, connectorDraggedOver);
        }
        #endregion


        private void CreateBlock_Executed(object sender, RoutedEventArgs e) {
            var mi = e.Source as MenuItem;
            CreateBlock(mi.Header as string);
        }

        private void CreateBlock(string type = "Simple") {
            var newNodeLocation = Mouse.GetPosition(networkControl);
            var block = NacFactory.CreateBlock(type, Section.Path, newNodeLocation);
            Engine.Add(block);
        }

        private void DeleteNode_Executed(object sender, ExecutedRoutedEventArgs e) {
            var node = (NacWpfBlockViewModel1)e.Parameter;
            if (node.AttachedConnections.Count == 0) Engine.Delete(node.Block.Path);
        }

        private void DeleteSelectedNodes_Executed(object sender, ExecutedRoutedEventArgs e) {
            var selNodes = ViewModel.Network.Nodes.Where(node => node.IsSelected).Where(node => node.AttachedConnections.Count == 0);
            Engine.Delete(selNodes.Select(node => node.Block.Path).ToArray());
        }

        private void DeleteConnection_Executed(object sender, ExecutedRoutedEventArgs e) {
            var connection = (NacWpfConnectionViewModel1)e.Parameter;
            try {
                Engine.Disconnect(connection.SourceConnector.ParentNode.Block.Path, connection.DestConnector.ParentNode.Block.Path, connection.SourceConnector.Name, connection.DestConnector.Name);
                ViewModel.Network.Connections.Remove(connection);
            } catch { }
        }

        private void Node_SizeChanged(object sender, SizeChangedEventArgs e) {
            //
            // The size of a node, as determined in the UI by the node's data-template,
            // has changed.  Push the size of the node through to the view-model.
            //
            var element = (FrameworkElement)sender;
            var node = (NacWpfBlockViewModel1)element.DataContext;
            node.Size = new Size(element.ActualWidth, element.ActualHeight);
        }

        public event SelectionChangedEventHandler BlockSelectionChanged;
        private void networkControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            SelectionChangedEventArgs scea = new SelectionChangedEventArgs(
                                                    e.RoutedEvent,
                                                    e.RemovedItems.Cast<NacWpfBlockViewModel1>().Select(m => m.Block).ToList(),
                                                    e.AddedItems.Cast<NacWpfBlockViewModel1>().Select(m => m.Block).ToList()
                                                    );
            BlockSelectionChanged?.Invoke(this, scea);
        }

        private void networkControl_NodeDragCompleted(object sender, NodeDragCompletedEventArgs e) {
            var pathsAndPositions =
                from node in e.Nodes.Cast<NacWpfBlockViewModel1>()
                select new { Path = node.Block.Path, Position = new Point(node.X, node.Y) };

            Engine.Update(pathsAndPositions.Select(pp => pp.Path).ToArray(), "Position", pathsAndPositions.Select(pp => pp.Position).Cast<object>().ToArray());
        }

        private void Refresh_Click(object sender, RoutedEventArgs e) {
            Refresh();
        }
        public void NacWpfRefresh() {
            if (Section.Online) Refresh();
        }

        private void Refresh() {
            var sectionViewModel = DataContext as NacWpfSectionViewModel1;
            Dictionary<string, NacExecutionStatus> states = Engine.GetSectionStates(Path);
            foreach (NacWpfBlock block in Blocks) block.Status = states[block.Path];
        }

        private void CopySectionContent(object sender, RoutedEventArgs e) {
            var selected = networkControl.SelectedNodes;
            NacClipboard<NacBlock[]>.Clear();
            NacClipboard<NacBlock[]>.Put(selected.OfType<NacWpfBlockViewModel1>().Select(blockViewModel => blockViewModel.Base).ToArray());
        }

        private void PasteSectionContent(object sender, RoutedEventArgs e) {
            var blocks = NacClipboard<NacBlock[]>.Get();
            Dictionary<string, string> pathReplace = new Dictionary<string, string>();
            foreach (var block in blocks) {
                var oldPath = block.Path; var newPath = NacPath.Parse(Path).Child(Guid.NewGuid().ToString());
                pathReplace[oldPath] = newPath;
            }
            foreach (var block in blocks) {
                block.Path = pathReplace[block.Path];
                Point pt = block.Position; pt.Offset(5, 5); block.Position = pt;
                if (block is NacBlockSeq) {
                    var blockSeq = block as NacBlockSeq;

                    var nexts = blockSeq.Next.ToArray();
                    blockSeq.Next.Clear();
                    foreach (var next in nexts) if (pathReplace.ContainsKey(next)) blockSeq.Next.Add(pathReplace[next]);

                    var prevs = blockSeq.Prev.ToArray();
                    blockSeq.Prev.Clear();
                    foreach (var prev in prevs) if (pathReplace.ContainsKey(prev)) blockSeq.Prev.Add(pathReplace[prev]);

                    if (block is NacBlockIf) {
                        var blockIf = block as NacBlockIf;

                        var nextTrues = blockIf.NextTrue.ToArray();
                        blockIf.NextTrue.Clear();
                        foreach (var nextTrue in nextTrues) if (pathReplace.ContainsKey(nextTrue)) blockIf.NextTrue.Add(pathReplace[nextTrue]);
                    }
                }
            }
            Engine.Add(blocks);
            foreach (var block in blocks) {
                var blockViewModel = networkControl.Nodes.OfType<NacWpfBlockViewModel1>().First(bvm => bvm.Base.Path == block.Path);
                networkControl.SelectedNodes.Add(blockViewModel);
            }
        }
    }

    public class ExecutionStatusColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var status = (NacExecutionStatus)value;

            if (status.Scheduled) {
                switch (status.Quality) {
                    case NacExecutionQuality.Good: return Colors.Green;
                    case NacExecutionQuality.BadCompilation: return Colors.Yellow;
                    case NacExecutionQuality.BadExecution: return Colors.Red;
                    default: return Colors.LightGray;
                }
            } else return Colors.LightGray;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return new NacExecutionStatus { Quality = NacExecutionQuality.Unknown };
        }
    }
}
