using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Somewhere2.WPFApplication.Applets
{
    public partial class TextEditor : Window, INotifyPropertyChanged
    {
        #region Construction
        public TextEditor(string text, Action<string> closed)
        {
            InitializeComponent();

            Text = text;
            ClosedResult = closed;
        }
        public Action<string> ClosedResult { get; set; }
        #endregion

        #region Public View Properties
        private string _Text = string.Empty;
        public string Text { get => _Text; set => SetField(ref _Text, value); }
        #endregion

        #region Events
        private void TextEditor_OnClosed(object sender, EventArgs e)
        {
            ClosedResult(Text);
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
    }
}