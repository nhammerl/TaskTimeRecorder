using System.Windows.Input;
using Windows.UI.Xaml;

namespace nhammerl.TTRecorder.ViewModel.Command
{
    /// <summary>
    /// Container for CommandViewmodel.
    /// </summary>
    public interface ICommandViewModel
    {
        // Displaytext from command.
        string Text { get; set; }

        // Imagepath if available.
        string ImagePath { get; set; }

        // Command to execute.
        ICommand Command { get; set; }

        // Acutal visibility.
        Visibility Visibility { get; set; }
    }
}