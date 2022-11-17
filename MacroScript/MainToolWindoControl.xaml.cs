using Community.VisualStudio.Toolkit;
using EnvDTE;
using EnvDTE80;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Design.Serialization;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VSLangProj;
using MessageBox = System.Windows.MessageBox;

namespace MacroScript
{
    /// <summary>
    /// Interaction logic for MainToolWindoControl.
    /// </summary>
    public partial class MainToolWindoControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainToolWindoControl"/> class.
        /// </summary>
        public MainToolWindoControl()
        {
            this.InitializeComponent();
            MainTextBox.Text = @"DTE.ExecuteCommand(""Help.About"");";
            if(!Directory.Exists(PathToUserTemplates))
            {
                Directory.CreateDirectory(PathToUserTemplates);
            }
            else
            {
                var defaultcs = Path.Combine(PathToUserTemplates, "Default.cs");
                if (File.Exists(defaultcs))
                {
                    MainTextBox.Text = File.ReadAllText(defaultcs);
                }
            }

        }
        private readonly string[] _imports =
    {
        "System", "System.Collections.Generic", "System.Diagnostics", "System.IO", "System.IO.Compression",
        "System.Text", "System.Text.RegularExpressions", "System.Threading.Tasks", "System.Linq",
        "System.Globalization", "EnvDTE"
    };
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var script = CSharpScript.Create(MainTextBox.Text,
                    ScriptOptions.Default
                    .WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                        .Where(xa => !xa.IsDynamic && !string.IsNullOrWhiteSpace(xa.Location))).WithImports(_imports),
                typeof(Global));
                var g = new Global();
                IComponentModel MyComponentModel = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
                SVsServiceProvider ServiceProvider = MyComponentModel.GetService<SVsServiceProvider>();
                g.DTE = (DTE)ServiceProvider.GetService(typeof(DTE));
                g.package = MainToolWindoCommand.Instance.package;
                var s = script.RunAsync(g);
            }
            catch (CompilationErrorException ex)
            {
                MessageBox.Show(string.Join("\n",ex.Diagnostics));
            }
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            var p = Path.Combine(PathToUserTemplates, fileNameBx.Text);
          
               File.WriteAllText(p,MainTextBox.Text);
        }

        private void LoadButton(object sender, RoutedEventArgs e)
        {
            var p = Path.Combine(PathToUserTemplates, fileNameBx.Text);
            if (File.Exists(p))
            {
                MainTextBox.Text = File.ReadAllText(p);
            }
        }
        string PathToUserTemplates = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MacroScript");
        private void addItem(string text)
        {
            //https://social.technet.microsoft.com/wiki/contents/articles/31190.autocomplete-textbox-in-wpf-without-third-party-libraries.aspx
            TextBlock block = new()
            {
                Text = text,

                Margin = new Thickness(2, 3, 2, 3),
                Cursor = Cursors.Hand
            };
            block.MouseLeftButtonUp += (sender, e) =>
            {
                fileNameBx.Text = (sender as TextBlock).Text;
            };
            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.PeachPuff;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };
            resultStack.Children.Add(block);
        }
        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            var data = Directory.GetFiles(PathToUserTemplates).Select(x=>Path.GetFileName(x));

            string query = (sender as TextBox).Text;

            if (query.Length == 0)
            {
                resultStack.Children.Clear();
                border.Visibility = Visibility.Collapsed;
            }
            else
            {
                border.Visibility = Visibility.Visible;
            }

            resultStack.Children.Clear();

            foreach (var obj in data)
            {
                if (obj.ToLower().StartsWith(query.ToLower()))
                {
                    addItem(obj);
                    found = true;
                }
            }

            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }

        private void fileNameBx_GotFocus(object sender, RoutedEventArgs e)
        {
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            border.Visibility = System.Windows.Visibility.Visible;
        }

        private void fileNameBx_LostFocus(object sender, RoutedEventArgs e)
        {
            Task.Delay(100).Wait();
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            border.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
    public class Global
    {
        public DTE DTE;
        public Package package;
    }
}
