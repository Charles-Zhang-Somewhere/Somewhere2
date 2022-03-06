using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace Somewhere2.WPFApplication.Applets
{
    public partial class ScratchPad : Window, INotifyPropertyChanged
    {
        public ScratchPad()
        {
            InitializeComponent();

            App = Application.Current as App;
            IsAddingMode = true;
            LabelContent = DefaultTextAdd;
        }
        private App App { get; }
        private const string DefaultTextAdd = "Drag Files Here to Tag";
        private const string DefaultTextRemove = "Drag Files Here to Untag";
        private string[] Tags => StringHelper.SplitTags(TagsList.Replace('<', ' '));
        
        #region Public View Properties
        private string _LabelContent = string.Empty;
        public string LabelContent { get => _LabelContent; set => SetField(ref _LabelContent, value); }
        private string _TagsList = string.Empty;
        public string TagsList { get => _TagsList; set => SetField(ref _TagsList, value); }
        private string _ToastLabelContent;
        public string ToastLabelContent { get => _ToastLabelContent; set => SetField(ref _ToastLabelContent, value); }
        private bool _IsAddingMode = true;
        public bool IsAddingMode { get => _IsAddingMode; set => SetField(ref _IsAddingMode, value); }
        private bool _IsDraggingOver = false;
        public bool IsDraggingOver { get => _IsDraggingOver; set => SetField(ref _IsDraggingOver, value); }
        #endregion

        #region Events
        private void ScratchPad_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void ScratchPad_OnDragLeave(object sender, DragEventArgs e)
        {
            IsDraggingOver = false;
        }
        private void ScratchPad_OnDragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) IsDraggingOver = false;
            else IsDraggingOver = true;
        }
        private void ScratchPad_OnDragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                IsDraggingOver = false;
            }
            else
            {
                IsDraggingOver = true;
            }
        }
        private void ScratchPad_OnDrop(object sender, DragEventArgs e)
        {
            string[] tags = Tags;
            if (tags.Length != 0)
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                RegisterItemTags(paths, tags);
                ToastLabelContent = $"{paths.Length} Updated";
            }
            else
            {
                ToastLabelContent = $"No Tags Are Specified";
            }
            
            ((Storyboard)FindResource("animate")).Begin(Toast);
        }
        #endregion
        
        #region Data Binding
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private bool SetField<TType>(ref TType field, TType value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TType>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
        #endregion

        private void ToggleAddTagsButton_OnClick(object sender, RoutedEventArgs e)
        {
            IsAddingMode = true;
            LabelContent = IsAddingMode ? DefaultTextAdd : DefaultTextRemove;
        }

        private void ToggleRemoveTagsButton_OnClick(object sender, RoutedEventArgs e)
        {
            IsAddingMode = false;
            LabelContent = IsAddingMode ? DefaultTextAdd : DefaultTextRemove;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TagsFieldTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string[] tags = Tags;
            if (tags.Length != 0)
                LabelContent = $"{(IsAddingMode ? DefaultTextAdd : DefaultTextRemove)}<LineBreak/><Span Style=\"{{DynamicResource Small}}\">(<Italic>{string.Join(", ", tags)}</Italic>)</Span>";
            else LabelContent = IsAddingMode ? DefaultTextAdd : DefaultTextRemove;
            
            // Update text block
            UpdateTextBlock(LabelContent);
        }

        #region Helpers
        public void UpdateTextBlock(string markupString)
        {
            // Example: var markupString = "foo bar <Bold>dong</Bold>";
            TextBlockContainer.Child = CreateTextBlock(markupString);
        }
        public TextBlock CreateTextBlock(string inlines)
        {
            var xaml = "<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">"
                       + inlines + "</TextBlock>";
            return XamlReader.Parse(xaml) as TextBlock;
        }
        #endregion

        #region Routines
        private void RegisterItemTags(string[] paths, string[] tags)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}