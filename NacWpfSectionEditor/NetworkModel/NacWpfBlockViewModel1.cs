using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Utils;
using System.Windows;
using Nac.Wpf.Common.Control;
using System.Reflection;
using Nac.Common.Control;
using System.ComponentModel;

namespace Nac.Wpf.SectionEditor.NetworkModel {
    /// <summary>
    /// Defines a node in the view-model.
    /// Nodes are connected to other nodes through attached connectors (aka anchor/connection points).
    /// </summary>
    public class NacWpfBlockViewModel1 : AbstractModelBase {
        #region Private Data Members

        private NacWpfBlock _block;

        private int zIndex = 0;

        /// <summary>
        /// The size of the node.
        /// 
        /// Important Note: 
        ///     The size of a node in the UI is not determined by this property!!
        ///     Instead the size of a node in the UI is determined by the data-template for the Node class.
        ///     When the size is computed via the UI it is then pushed into the view-model
        ///     so that our application code has access to the size of a node.
        /// </summary>
        private Size size = Size.Empty;

        /// <summary>
        /// List of input connectors (connections points) attached to the node.
        /// </summary>
        private ImpObservableCollection<NacWpfConnectorViewModel1> inputConnectors = null;

        /// <summary>
        /// List of output connectors (connections points) attached to the node.
        /// </summary>
        private ImpObservableCollection<NacWpfConnectorViewModel1> outputConnectors = null;

        /// <summary>
        /// Set to 'true' when the node is selected.
        /// </summary>
        private bool isSelected = false;

        #endregion Private Data Members

        public NacWpfBlock Block { get { return _block; }
            private set {
                _block = value;
                Position = Base.Position;
                Block.PropertyChanged += Block_PropertyChanged;
            }
        }
        public NacBlock Base { get { return _block.Base; } }

        public NacWpfBlockViewModel1() {}

        public NacWpfBlockViewModel1(NacWpfBlock block) {
            Block = block;
        }

        private void Block_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            //PropertyInfo piDst = GetType().GetProperty(e.PropertyName);
            //if (piDst != null) {
            //    PropertyInfo piSrc = sender.GetType().GetProperty(e.PropertyName);
            //    piDst.SetValue(this, piSrc.GetValue(sender));
            //}
            OnPropertyChanged(e.PropertyName);
        }

        public string Description { get { return Block.Description; } set { if (value != Block.Description) { Block.Description = value; OnPropertyChanged("Description"); } } }
        //public NacExecutionStatus Status { get { return Block.Status; } set { if (value != Block.Status) { Block.Status = value; OnPropertyChanged("Status"); } } }
        //public NacExecutionQuality Quality { get { return Status.Quality; } set { if (value != Block.Quality) { Block.Quality = value; OnPropertyChanged("Quality"); } } }

        //public string Name { get { return Block.Name; } set { Block.Name = value; } }
        public NacExecutionStatus Status { get { return Block.Status; } set { Block.Status = value;} }
        public NacExecutionQuality Quality { get { return Status.Quality; } set { Block.Quality = value; } }
        public TimeSpan Countdown { get { return Status.Countdown; } set { Block.Countdown = value; } }

        private double _x, _y;
        public double X { get { return _x; } set { _x = value; OnPropertyChanged("X"); } }
        public double Y { get { return _y; } set { _y = value;  OnPropertyChanged("Y"); } }

        public Point Position { set { X = value.X; Y = value.Y; } }
        /// <summary>
        /// The Z index of the node.
        /// </summary>
        public int ZIndex
        {
            get
            {
                return zIndex;
            }
            set
            {
                if (zIndex == value)
                {
                    return;
                }

                zIndex = value;

                OnPropertyChanged("ZIndex");
            }
        }

        /// <summary>
        /// The size of the node.
        /// 
        /// Important Note: 
        ///     The size of a node in the UI is not determined by this property!!
        ///     Instead the size of a node in the UI is determined by the data-template for the Node class.
        ///     When the size is computed via the UI it is then pushed into the view-model
        ///     so that our application code has access to the size of a node.
        /// </summary>
        public Size Size
        {
            get
            {
                return size;
            }
            set
            {
                if (size == value)
                {
                    return;
                }

                size = value;

                if (SizeChanged != null)
                {
                    SizeChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Event raised when the size of the node is changed.
        /// The size will change when the UI has determined its size based on the contents
        /// of the nodes data-template.  It then pushes the size through to the view-model
        /// and this 'SizeChanged' event occurs.
        /// </summary>
        public event EventHandler<EventArgs> SizeChanged;

        /// <summary>
        /// List of input connectors (connections points) attached to the node.
        /// </summary>
        public ImpObservableCollection<NacWpfConnectorViewModel1> InputConnectors
        {
            get
            {
                if (inputConnectors == null)
                {
                    inputConnectors = new ImpObservableCollection<NacWpfConnectorViewModel1>();
                    inputConnectors.ItemsAdded += new EventHandler<CollectionItemsChangedEventArgs>(inputConnectors_ItemsAdded);
                    inputConnectors.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(inputConnectors_ItemsRemoved);
                }

                return inputConnectors;
            }
        }

        /// <summary>
        /// List of output connectors (connections points) attached to the node.
        /// </summary>
        public ImpObservableCollection<NacWpfConnectorViewModel1> OutputConnectors
        {
            get
            {
                if (outputConnectors == null)
                {
                    outputConnectors = new ImpObservableCollection<NacWpfConnectorViewModel1>();
                    outputConnectors.ItemsAdded += new EventHandler<CollectionItemsChangedEventArgs>(outputConnectors_ItemsAdded);
                    outputConnectors.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(outputConnectors_ItemsRemoved);
                }

                return outputConnectors;
            }
        }

        /// <summary>
        /// A helper property that retrieves a list (a new list each time) of all connections attached to the node. 
        /// </summary>
        public ICollection<NacWpfConnectionViewModel1> AttachedConnections
        {
            get
            {
                List<NacWpfConnectionViewModel1> attachedConnections = new List<NacWpfConnectionViewModel1>();

                foreach (var connector in this.InputConnectors)
                {
                    attachedConnections.AddRange(connector.AttachedConnections);
                }

                foreach (var connector in this.OutputConnectors)
                {
                    attachedConnections.AddRange(connector.AttachedConnections);
                }

                return attachedConnections;
            }
        }

        /// <summary>
        /// Set to 'true' when the node is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected == value)
                {
                    return;
                }

                isSelected = value;

                OnPropertyChanged("IsSelected");
            }
        }

        #region Private Methods

        /// <summary>
        /// Event raised when connectors are added to the node.
        /// </summary>
        private void inputConnectors_ItemsAdded(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (NacWpfConnectorViewModel1 connector in e.Items)
            {
                connector.ParentNode = this;
                connector.Type = NacWpfConnectorType.Input;
            }
        }

        /// <summary>
        /// Event raised when connectors are removed from the node.
        /// </summary>
        private void inputConnectors_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (NacWpfConnectorViewModel1 connector in e.Items)
            {
                connector.ParentNode = null;
                connector.Type = NacWpfConnectorType.Undefined;
            }
        }

        /// <summary>
        /// Event raised when connectors are added to the node.
        /// </summary>
        private void outputConnectors_ItemsAdded(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (NacWpfConnectorViewModel1 connector in e.Items)
            {
                connector.ParentNode = this;
                connector.Type = NacWpfConnectorType.Output;
            }
        }

        /// <summary>
        /// Event raised when connectors are removed from the node.
        /// </summary>
        private void outputConnectors_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (NacWpfConnectorViewModel1 connector in e.Items)
            {
                connector.ParentNode = null;
                connector.Type = NacWpfConnectorType.Undefined;
            }
        }

        #endregion Private Methods
    }
}
