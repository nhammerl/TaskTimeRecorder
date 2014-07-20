// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using nhammerl.TTRecorder.ViewModel;
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
            _mainViewModel = new MainViewModel();

            this.DataContext = _mainViewModel;
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

        private void SelectionChangedOnTaskListSetSelectedItemToNull(object sender, SelectionChangedEventArgs e)
        {
            TaskList.SelectedItem = null;
        }

        private void InputFieldKeyUpFocusButtonOnEnter(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) { return; }

            _mainViewModel.CloseInputDialog.Command.Execute(null);
        }
    }
}