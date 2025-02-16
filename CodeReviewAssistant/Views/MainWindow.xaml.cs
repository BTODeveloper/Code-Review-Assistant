using System.Windows;
using System.Windows.Controls;
using CodeReviewAssistant.ViewModels;

namespace CodeReviewAssistant
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            
            if (DataContext is MainViewModel vm)
            {
                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(MainViewModel.CurrentFile))
                    {
                        if (vm.CurrentFile != null)
                        {
                            DiffViewer.Text = vm.CurrentFile.DiffContent ?? string.Empty;
                        }
                    }
                };
            }
        }

        private void TreeView_OnSelectedItemChanged(object sender, 
            RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SelectedFileChanged(e.NewValue as CodeFileViewModel);
            }
        }
    }
}