using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using WinRT.Interop;

namespace Odyssey.Helpers
{
    // This was made specifically for the Odyssey and LightCode titlebars but should works on almost every app without modification
    public class TitleBarDragRegions // v1
    {
        [DllImport("Shcore.dll", SetLastError = true)]
        public static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

        public enum Monitor_DPI_Type : int
        {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default = MDT_Effective_DPI
        }

        public double GetScaleAdjustment()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(_window);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
            IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

            // Get DPI.
            int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _);
            if (result != 0)
            {
                throw new System.Exception("Could not get DPI for monitor.");
            }

            uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
            return scaleFactorPercent / 100.0;
        }

        // Test if the titlebar is inside of a closed or opened StackPanel
        private static bool IsVisible(FrameworkElement element)
        {
            try
            {
                var parent = VisualTreeHelper.GetParent(element);
                int tests = 0;
                while (parent.GetType() != typeof(SplitView) && tests < 10) {
                    tests++;
                    parent = VisualTreeHelper.GetParent(parent);
                }

                if (parent.GetType() == typeof(SplitView)) {
                    SplitView splitView = (SplitView)parent;
                    return splitView.IsPaneOpen;
                }
                else return true;
            }
            catch { return true; }
        }



        private List<Grid> _titleBars; // each titleBar should have 1+ not type-whitelisted control
        private Window _window = null;
        private List<Type> _whiteList; // every control type which will be ignored when the drag regions are set (TextBlock,...)
        private FrameworkElement _titleBarParent; // this should be Window.Content in most cases
        private int _height;

        /// <remarks>
        /// Every Grid and StackPanel used should have fixed value or HorizontalAlignement
        /// </remarks>
        /// <summary>
        /// Object that automatically create titleBar drag regions according to various parameters.
        /// </summary>
        /// <param name="titleBars">A list of every 'titlebar' used for the drag regions.</param>
        /// <param name="whiteList">Every type which won't be taken into account when creating the drag regions</param>
        /// <param name="titleBarParent">The parent of every titlebar. Should be Window.Content in most cases</param>
        /// <param name="height">The height of the drag regions</param>
        public TitleBarDragRegions(List<Grid> titleBars, Window window, List<Type> whiteList, FrameworkElement titleBarParent, int height)
        {
            _titleBars = titleBars;
            _window = window;
            _whiteList = whiteList;
            _titleBarParent = titleBarParent;
            _height = height;

            foreach (var titleBar in titleBars)
            {
                titleBar.Loaded += (s, a) => SetDragRegionForTitleBars();
                titleBar.SizeChanged += (s, a) => SetDragRegionForTitleBars();
            }
        }



        
        private Windows.Graphics.RectInt32 GetFirstOrLastDragRegionForTitleBar(Grid titleBar, bool first = true)
        {
            var children = titleBar.Children.ToList().Where(p => !_whiteList.Contains(p.GetType())).ToList();

            var transformTitleBar = titleBar.TransformToVisual(_titleBarParent);
            Point titleBarPosition = transformTitleBar.TransformPoint(new Point(0, 0));

            if (first)
            {
                double scaleAdjustment = GetScaleAdjustment();

                UIElement firstChild = children[0];
                var transform = firstChild.TransformToVisual(titleBar);
                Point childPosition = transform.TransformPoint(new Point(0, 0));

                // Create the actual drag region based on the elements 1 and 2 and the titleBar position
                Windows.Graphics.RectInt32 dragRect;
                dragRect.X = (int)(titleBarPosition.X * scaleAdjustment);
                dragRect.Y = 0;
                dragRect.Height = (int)(_height * scaleAdjustment);


                if (IsVisible(firstChild as FrameworkElement))
                    dragRect.Width = (int)(childPosition.X * scaleAdjustment);
                else
                    dragRect.Width = 0;

                return dragRect;
            }
            else
            {
                double scaleAdjustment = GetScaleAdjustment();

                UIElement firstChild = children.Last();
                var transform = firstChild.TransformToVisual(titleBar);
                Point childPosition = transform.TransformPoint(new Point(0, 0));

                // Create the actual drag region based on the elements 1 and 2 and the titleBar position
                Windows.Graphics.RectInt32 dragRect;
                dragRect.X = (int)((titleBarPosition.X + firstChild.ActualSize.X + childPosition.X) * scaleAdjustment);
                dragRect.Y = 0;
                dragRect.Height = (int)(_height * scaleAdjustment);
                dragRect.Width = (int)((titleBar.ActualWidth - (firstChild.ActualSize.X + childPosition.X)) * scaleAdjustment);

                return dragRect;
            }
        }



        
        public void SetDragRegionForTitleBars()
        {
            List<Windows.Graphics.RectInt32> dragRectsList = new();

            foreach (var titleBar in _titleBars)
            {
                var transformTitleBar = titleBar.TransformToVisual(_titleBarParent);
                Point titleBarPosition = transformTitleBar.TransformPoint(new Point(0, 0));

                double scaleAdjustment = GetScaleAdjustment();

                var children = titleBar.Children.ToList().Where(p => !_whiteList.Contains(p.GetType())).ToList();
                if(children.Count > 0)
                {
                    // Set the first drag region (between the start of the titlebar and the first control)
                    dragRectsList.Add(GetFirstOrLastDragRegionForTitleBar(titleBar));
    
                    for (int i = 0; i < children.Count - 1; i++)
                    {
                        // Get the two element which in between the drag region will be setted
                        UIElement element1 = children[i];
                        UIElement element2 = children[i + 1];
    
                        // Get the position of the two element relative to the titlebar
                        var transform = element1.TransformToVisual(titleBar);
                        var transform2 = element2.TransformToVisual(titleBar);
    
                        // Convert to point
                        Point element1Position = transform.TransformPoint(new Point(0, 0));
                        Point element2Position = transform2.TransformPoint(new Point(0, 0));

                        // Get the max width value between the 2 elements to avoid negative values
                        int width = Math.Max((int)((element2Position.X - (element1Position.X + element1.ActualSize.X)) * scaleAdjustment),
                                             (int)((element1Position.X - (element2Position.X + element2.ActualSize.X)) * scaleAdjustment));

                        // Idem but for too high values
                        int x = Math.Min((int)((element1.ActualSize.X + titleBarPosition.X) * scaleAdjustment),
                                         (int)((element2.ActualSize.X + titleBarPosition.X) * scaleAdjustment));

                        // Create the actual drag region based on the elements 1 and 2 and the titleBar position
                        Windows.Graphics.RectInt32 dragRect;
                        dragRect.X = x;
                        dragRect.Y = 0;
                        dragRect.Height = (int)(_height * scaleAdjustment);
                        dragRect.Width = width;
                        dragRectsList.Add(dragRect);
                    }
    
                    // Set the last titleBar (between the last control and the end of the titlebar)
                    dragRectsList.Add(GetFirstOrLastDragRegionForTitleBar(titleBar, false));
                }
                else // When no element should be manipulated by the user
                {
                    // Create a drag region for the entire titlebar
                    Windows.Graphics.RectInt32 dragRect;
                    dragRect.X = (int)(titleBarPosition.X * scaleAdjustment);
                    dragRect.Y = 0;
                    dragRect.Height = (int)(_height * scaleAdjustment);
                    dragRect.Width = (int)(titleBar.ActualWidth * scaleAdjustment);
                    dragRectsList.Add(dragRect);
                }
            }

            Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();
            _window.AppWindow.TitleBar.SetDragRectangles(dragRects);
        }
    }
}
