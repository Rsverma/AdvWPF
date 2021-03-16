using AdvWPF.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdvWPF.Controls
{
    public class AdvDataGrid : DataGrid
    {
        static AdvDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AdvDataGrid), new FrameworkPropertyMetadata(typeof(DataGrid)));
        }

        private ObservableCollection<DataGridColumn> _columns;

        public AdvDataGrid()
            : base()
        {
            ColumnChooserClicked = new RelayCommand(OnColumnChooserClicked, CanChooseColumns);
            Loaded += AdvDataGrid_Loaded;
        }

        private void AdvDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as AdvDataGrid;
            _columns = grid.Columns;
            ReplaceSelectAllButton(this);
            Loaded -= AdvDataGrid_Loaded;
        }

        void ReplaceSelectAllButton(DependencyObject dependencyObject)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);
                if(child!=null)
                {

                    if (child is Button button)
                    {
                        button.Command = ColumnChooserClicked;
                    }
                    else
                    {
                        ReplaceSelectAllButton(child);
                    }
                }
            }
        }
        public ICommand ColumnChooserClicked { get; set; }
        private AdvColumnChooser columnChooser;
        private bool CanChooseColumns(object value)
        {
            return true;
        }


        void OnColumnChooserClicked(object value)
        {
            if (columnChooser == null)
            {
                BindingList<ColumnVisibility> columnsVisiblityMapping = new BindingList<ColumnVisibility>();
                foreach (var col in _columns)
                {
                    columnsVisiblityMapping.Add(new ColumnVisibility { Name = col.Header.ToString(), IsVisible = col.Visibility == Visibility.Visible });
                }
                columnChooser = new AdvColumnChooser
                {
                    Owner = Window.GetWindow(this),
                    ColumnsVisiblityMapping = columnsVisiblityMapping
                };

                columnChooser.CheckedUnChecked += Win_CheckedUnChecked;
                columnChooser.Closing += Win_Closing;
                columnChooser.Show();
            }
            else
            {
                columnChooser.Activate();
            }
        }

        private void Win_Closing(object sender, CancelEventArgs e)
        {
            columnChooser.CheckedUnChecked -= Win_CheckedUnChecked;
            columnChooser.Closing -= Win_Closing;
            columnChooser = null;
        }

        private void Win_CheckedUnChecked(object sender, RoutedEventArgs e)
        {
            var chkBox = sender as CheckBox;
            string columnName = chkBox.Content.ToString();

            DataGridColumn changedColumn = _columns.FirstOrDefault(c => c.Header.ToString().Equals(columnName));
            if (changedColumn != null)
            {
                Visibility visibility = e.RoutedEvent.Name.Equals("Checked") ? Visibility.Visible : Visibility.Collapsed;
                changedColumn.Visibility = visibility;
            }
        }
    }
}
