using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;

using EaseFilter.GlobalObjects;
using EaseFilter.CloudManager;

namespace EaseFilter.CloudExplorer
{
    /// <summary>
    /// 
    /// </summary>
    public class ListViewComparer : IComparer
    {
        #region Private variables
    
        public enum Column:int
        {
         COLUMN_NAME = 0,
         COLUMN_SIZE,
         COLUMN_DATE,
        }

        Column column = Column.COLUMN_NAME;
        SortOrder sortOrder = SortOrder.Ascending;
      
        #endregion

        #region Constructor
     
        /// <summary>
        /// Creates an instance of the AEListViewComparer class.
        /// </summary>
        /// <param name="column">The column number to sort.</param>
        public ListViewComparer(Column column, SortOrder sort)
        {
            this.column = column;
            sortOrder = sort;
        }
        #endregion

        #region IComparer Members

        int IComparer.Compare(object x, object y)
        {
            int ret = 0;

            try
            {
                if (column == Column.COLUMN_NAME)
                {
                    string x1 = ((ListViewItem)x).SubItems[(int)column].Text;
                    string y1 = ((ListViewItem)y).SubItems[(int)column].Text;
                    switch (sortOrder)
                    {
                        case SortOrder.Ascending:
                            ret = string.Compare(x1,y1);
                            break;

                        case SortOrder.Descending:
                            ret = string.Compare(y1, x1);
                            break;

                        case SortOrder.None:
                        default:
                            ret = 0;
                            break;
                    }
                }
                else if (column == Column.COLUMN_SIZE)
                {
                    long x1 = ((FileEntry)((ListViewItem)x).Tag).FileSize;
                    long y1 = ((FileEntry)((ListViewItem)y).Tag).FileSize;

                    switch (sortOrder)
                    {
                        case SortOrder.Ascending:
                            ret = (y1 > x1) ? 1 : -1;
                            break;

                        case SortOrder.Descending:
                            ret = (y1 > x1) ? -1 : 1;
                            break;

                        case SortOrder.None:
                        default:
                            ret = 0;
                            break;
                    }
                }
                else if (column == Column.COLUMN_DATE)
                {
                    long x1 = ((FileEntry)((ListViewItem)x).Tag).LastWriteTime;
                    long y1 = ((FileEntry)((ListViewItem)y).Tag).LastWriteTime;

                    switch (sortOrder)
                    {
                        case SortOrder.Ascending:
                            ret = (y1 > x1) ? 1 : -1;
                            break;

                        case SortOrder.Descending:
                            ret = (y1 > x1) ? -1 : 1;
                            break;

                        case SortOrder.None:
                        default:
                            ret = 0;
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                EventManager.WriteMessage(1401, "Comparer", EventLevel.Error, ex.Message);
            }

            return ret;
        }
        #endregion
    }

}
