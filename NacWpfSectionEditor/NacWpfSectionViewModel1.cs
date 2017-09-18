using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Nac.Wpf.SectionEditor.NetworkModel;
using System.Windows;
using System.Diagnostics;
using Nac.Wpf.Common.Control;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Globalization;
using System.Reflection;
using Nac.Wpf.Common;

namespace Nac.Wpf.SectionEditor {
    public class NacWpfSectionViewModel1 : AbstractModelBase {
        #region Internal Data Members

        /// <summary>
        /// This is the network that is displayed in the window.
        /// It is the main part of the view-model.
        /// </summary>
        public NacWpfNetworkViewModel1 network = null;

        ///
        /// The current scale at which the content is being viewed.
        /// 
        private double contentScale = 1;

        ///
        /// The X coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        private double contentOffsetX = 0;

        ///
        /// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        private double contentOffsetY = 0;

        ///
        /// The width of the content (in content coordinates).
        /// 
        private double contentWidth = 2000;

        ///
        /// The heigth of the content (in content coordinates).
        /// 
        private double contentHeight = 1000;

        ///
        /// The width of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// view-model so that the value can be shared with the overview window.
        /// 
        private double contentViewportWidth = 0;

        ///
        /// The height of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// view-model so that the value can be shared with the overview window.
        /// 
        private double contentViewportHeight = 0;

        #endregion Internal Data Members

        public NacWpfSectionViewModel1() { Network = new NacWpfNetworkViewModel1(); }
        public NacWpfSectionViewModel1(NacWpfSection section) : this() { Section = section; }

        private void Section_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            PropertyInfo piDst = GetType().GetProperty(e.PropertyName);
            if (piDst != null) {
                PropertyInfo piSrc = sender.GetType().GetProperty(e.PropertyName);
                piDst.SetValue(this, piSrc.GetValue(sender));
            }
        }

        private void Section_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) AddBlocks(e.NewItems.Cast<NacWpfObject>());
            else
            if (e.Action == NotifyCollectionChangedAction.Remove) RemoveBlocks(e.OldItems.Cast<NacWpfObject>());
        }

        NacWpfSection _section;
        public NacWpfSection Section {
            get { return _section; }
            set {
                _section = value;
                ResetNetwork();
                AddBlocks(Section.Blocks);
                Section.CollectionChanged += Section_CollectionChanged;
                Section.PropertyChanged += Section_PropertyChanged;
            }
        }

        public bool Online { get { return Section.Online; } set { if (value != Section.Online) { Section.Online = value; OnPropertyChanged("Online"); } } }

        private void ResetNetwork() {
            Network.Nodes.Clear();
            Network.Connections.Clear();
        }

        private void AddBlocks(IEnumerable<NacWpfObject> blocks) {
            foreach (var block in blocks.OfType<NacWpfBlock>()) CreateNode(block);
            foreach (var block in blocks.OfType<NacWpfBlockSeq>()) CreateConnections(block);
        }

        private void RemoveBlocks(IEnumerable<NacWpfObject> blocks) {
            foreach (var block in blocks.OfType<NacWpfBlockSeq>()) DeleteConnections(block);
            foreach (var block in blocks.OfType<NacWpfBlock>()) DeleteNode(block);
        }

        public NacWpfBlockViewModel1 CreateNode(NacWpfBlock block) {
            var viewModelType = Type.GetType($"{typeof(NacWpfBlockViewModel1).Namespace}.{block.GetType().Name}ViewModel1");
            var node = Activator.CreateInstance(viewModelType, block) as NacWpfBlockViewModel1;
            Network.Nodes.Add(node);
            
            return node;
        }


        public void DeleteNode(NacWpfBlock block) {
            var node = FindNode(block.Path);
            Network.Nodes.Remove(node);
        }

        private void CreateConnections(NacWpfBlockSeq block) {
            var src = FindNode(block.Path) as NacWpfBlockSeqViewModel1;
            foreach(var dest in src.GetDestinations()) {
                var dst = FindNode(dest.Item2);
                if (dst != null) CreateConnection(src, dst, dest.Item1, dest.Item3);
            }
        }

        private void CreateConnection(NacWpfBlockViewModel1 src, NacWpfBlockViewModel1 dst, string sourceConnector, string destinationConnector) {
            var connection = new NacWpfConnectionViewModel1();

            connection.SourceConnector = src.OutputConnectors.First(connector => connector.Name == sourceConnector);
            connection.DestConnector = dst.InputConnectors.First(connector => connector.Name == destinationConnector);
            Network.Connections.Add(connection);
        }

        private void DeleteConnections(NacWpfBlockSeq block) {
            var node = FindNode(block.Path);
            var connections = node?.AttachedConnections;
            Network.Connections.RemoveRange(connections);
        }
        private void DeleteConnection(NacWpfBlockViewModel1 src, NacWpfBlockViewModel1 dst) {
            var connection = FindConnection(src.OutputConnectors[0], dst.InputConnectors[0]);
            Network.Connections.Remove(connection);
        }

        public NacWpfNetworkViewModel1 Network {
            get { return network; }
            set {
                network = value;
                OnPropertyChanged("Network");
            }
        }

        #region sizing
        public double ContentScale {
            get { return contentScale; }
            set {
                contentScale = value;
                OnPropertyChanged("ContentScale");
            }
        }

        public double ContentOffsetX {
            get { return contentOffsetX; }
            set {
                contentOffsetX = value;
                OnPropertyChanged("ContentOffsetX");
            }
        }

        public double ContentOffsetY {
            get { return contentOffsetY; }
            set {
                contentOffsetY = value;
                OnPropertyChanged("ContentOffsetY");
            }
        }

        public double ContentWidth {
            get { return contentWidth; }
            set {
                contentWidth = value;
                OnPropertyChanged("ContentWidth");
            }
        }

        public double ContentHeight {
            get { return contentHeight; }
            set {
                contentHeight = value;
                OnPropertyChanged("ContentHeight");
            }
        }

        public double ContentViewportWidth {
            get { return contentViewportWidth; }
            set {
                contentViewportWidth = value;
                OnPropertyChanged("ContentViewportWidth");
            }
        }

        public double ContentViewportHeight {
            get { return contentViewportHeight; }
            set {
                contentViewportHeight = value;
                OnPropertyChanged("ContentViewportHeight");
            }
        }
        #endregion

        #region connection dragging
        public NacWpfConnectionViewModel1 ConnectionDragStarted(NacWpfConnectorViewModel1 draggedOutConnector, Point curDragPoint) {
            var connection = new NacWpfConnectionViewModel1();

            if (draggedOutConnector.Type == NacWpfConnectorType.Output) {
                connection.SourceConnector = draggedOutConnector;
                connection.DestConnectorHotspot = curDragPoint;
            }
            else {
                connection.DestConnector = draggedOutConnector;
                connection.SourceConnectorHotspot = curDragPoint;
            }

            Network.Connections.Add(connection);
            return connection;
        }

        public void QueryConnnectionFeedback(NacWpfConnectorViewModel1 draggedOutConnector, NacWpfConnectorViewModel1 draggedOverConnector, out object feedbackIndicator, out bool connectionOk) {
            if (draggedOutConnector == draggedOverConnector) {
                feedbackIndicator = new ConnectionBadIndicator();
                connectionOk = false;
            }
            else {
                var sourceConnector = draggedOutConnector;
                var destConnector = draggedOverConnector;
                connectionOk = sourceConnector.ParentNode != destConnector.ParentNode && sourceConnector.Type != destConnector.Type;

                if (connectionOk) { feedbackIndicator = new ConnectionOkIndicator(); }
                else { feedbackIndicator = new ConnectionBadIndicator(); }
            }
        }

        public void ConnectionDragging(Point curDragPoint, NacWpfConnectionViewModel1 connection) {
            if (connection.DestConnector == null) { connection.DestConnectorHotspot = curDragPoint; }
            else { connection.SourceConnectorHotspot = curDragPoint; }
        }

        public void ConnectionDragCompleted(NacWpfConnectionViewModel1 newConnection, NacWpfConnectorViewModel1 connectorDraggedOut, NacWpfConnectorViewModel1 connectorDraggedOver) {
            if (connectorDraggedOver == null) {
                Network.Connections.Remove(newConnection);
                return;
            }

            bool connectionOk = connectorDraggedOut.ParentNode != connectorDraggedOver.ParentNode && connectorDraggedOut.Type != connectorDraggedOver.Type;

            if (!connectionOk) {
                Network.Connections.Remove(newConnection);
                return;
            }

            var existingConnection = FindConnection(connectorDraggedOut, connectorDraggedOver);
            if (existingConnection != null) {
                Network.Connections.Remove(newConnection);
                return;
            }

            try {
                NacWpfConnectorViewModel1 src = connectorDraggedOut.Type == NacWpfConnectorType.Output ? connectorDraggedOut : connectorDraggedOver;
                NacWpfConnectorViewModel1 dst = connectorDraggedOut.Type == NacWpfConnectorType.Input ? connectorDraggedOut : connectorDraggedOver;

                Section.Engine.Connect(src.ParentNode.Block.Path, dst.ParentNode.Block.Path, src.Name, dst.Name);
                if (newConnection.DestConnector == null) { newConnection.DestConnector = connectorDraggedOver; }
                else { newConnection.SourceConnector = connectorDraggedOver; }
            }
            catch { Network.Connections.Remove(newConnection); }
        }
        #endregion

        public NacWpfBlockViewModel1 FindNode(string path) {
            return Network.Nodes.FirstOrDefault(m => m.Block.Path == path);
        }

        public NacWpfConnectionViewModel1 FindConnection(NacWpfConnectorViewModel1 connector1, NacWpfConnectorViewModel1 connector2) {
            Trace.Assert(connector1.Type != connector2.Type);
            var sourceConnector = connector1.Type == NacWpfConnectorType.Output ? connector1 : connector2;
            var destConnector = connector1.Type == NacWpfConnectorType.Output ? connector2 : connector1;

            foreach (var connection in sourceConnector.AttachedConnections) {
                if (connection.DestConnector == destConnector) return connection;
            }
            return null;
        }

        //public void DeleteSelectedNodes() {
        //    // Take a copy of the selected nodes list so we can delete nodes while iterating.
        //    var nodesCopy = Network.Nodes.ToArray();
        //    foreach (var node in nodesCopy) {
        //        if (node.IsSelected) DeleteNode(node);
        //    }
        //}
    }

    public class NacWpfSectionViewConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            NacWpfSection section = value as NacWpfSection;
            if (section != null) return new NacWpfSectionViewModel1(section);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return (value as NacWpfSectionViewModel1)?.Section;
        }
    }
}
