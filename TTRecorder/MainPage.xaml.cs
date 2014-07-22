// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using nhammerl.TTRecorder.ViewModel;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace nhammerl.TTRecorder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly MainViewModel _mainViewModel;

        public MainPage()
        {
            this.InitializeComponent();
            _mainViewModel = new MainViewModel(this);

            this.SizeChanged += MainPage_SizeChanged;
            this.DataContext = _mainViewModel;
        }

        /// <summary>
        /// Workaround to change the width of all items on changing the with of the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var task in TaskList.Items.Cast<ITaskViewModel>())
            {
                task.ItemVisualWidth = TaskList.ActualWidth;
            }
        }

        /// <summary>
        /// Set focus to input textbox after showing the popup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FocusTextBoxOnopen(object sender, object e)
        {
            InputFromInputDialog.Focus(FocusState.Keyboard);
        }

        /// <summary>
        /// Disable the selected item highlighting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionChangedOnTaskListSetSelectedItemToNull(object sender, SelectionChangedEventArgs e)
        {
            TaskList.SelectedItem = null;
        }

        /// <summary>
        /// Enable create task on press enter in popup mask.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputFieldKeyUpFocusButtonOnEnter(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) { return; }

            _mainViewModel.CloseInputDialog.Command.Execute(null);
        }
    }
}