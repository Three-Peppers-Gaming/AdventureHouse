using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terminal.Gui;
using AdventureRealms.Services.AdventureClient.Models;

namespace AdventureRealms.Services.AdventureClient.UI
{
    /// <summary>
    /// Terminal.Gui-based renderer for the adventure game map.
    /// This class is responsible for converting MapModel data into Terminal.Gui visual elements.
    /// Key Terminal.Gui concepts used:
    /// - View: Base class for all UI elements that can be displayed
    /// - FrameView: A view with a border and title
    /// - ColorScheme: Defines colors for different UI states
    /// - Application: Main application class that manages the UI loop
    /// </summary>
    public class TerminalGuiRenderer
    {
        /// <summary>
        /// Color scheme for visited rooms (green theme)
        /// </summary>
        private static readonly ColorScheme VisitedRoomScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.BrightGreen, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.Green, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.BrightGreen, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.DarkGray, Color.Black)
        };

        /// <summary>
        /// Color scheme for the current room (cyan theme)
        /// </summary>
        private static readonly ColorScheme CurrentRoomScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.BrightCyan, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.BrightCyan, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.DarkGray, Color.Black)
        };

        /// <summary>
        /// Color scheme for room with items (white theme - closest to yellow available)
        /// </summary>
        private static readonly ColorScheme ItemRoomScheme = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Black),
            HotNormal = new Terminal.Gui.Attribute(Color.White, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Black),
            Disabled = new Terminal.Gui.Attribute(Color.DarkGray, Color.Black)
        };

        /// <summary>
        /// Render the map to an ASCII string suitable for display in a TextView.
        /// This method creates a 2D character array representation of the map.
        /// </summary>
        /// <param name="mapModel">The map data to render</param>
        /// <param name="width">Maximum width for the rendered map</param>
        /// <param name="height">Maximum height for the rendered map</param>
        /// <returns>ASCII string representation of the map</returns>
        public string RenderMapToString(MapModel mapModel, int width = 80, int height = 40)
        {
            var visitedRooms = mapModel.GetVisitedRoomsForCurrentLevel();
            
            if (!visitedRooms.Any())
            {
                return "No rooms visited on this level yet.\n\nMove around to explore and reveal the map!";
            }

            // Calculate map bounds based on visited rooms
            var bounds = mapModel.GetLevelBounds(mapModel.CurrentLevel);
            
            // Calculate scaling to fit within available space
            var roomsWidth = bounds.MaxX - bounds.MinX + 1;
            var roomsHeight = bounds.MaxY - bounds.MinY + 1;
            
            // Each room takes 4x3 characters (box drawing)
            var neededWidth = roomsWidth * 4;
            var neededHeight = roomsHeight * 3;
            
            // Ensure we don't exceed available space
            var mapWidth = Math.Min(neededWidth, width - 2); // Leave space for borders
            var mapHeight = Math.Min(neededHeight, height - 2);
            
            // Create the character grid
            var map = new char[mapHeight, mapWidth];
            
            // Fill with spaces
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    map[y, x] = ' ';
                }
            }
            
            // Draw connections first (so rooms draw over them)
            DrawConnectionsOnMap(map, visitedRooms, mapModel, bounds.MinX, bounds.MinY, mapWidth, mapHeight);
            
            // Draw room boxes
            foreach (var room in visitedRooms)
            {
                DrawRoomBoxOnMap(map, room, bounds.MinX, bounds.MinY, mapWidth, mapHeight);
            }
            
            // Convert to string
            var result = new StringBuilder();
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    result.Append(map[y, x]);
                }
                if (y < mapHeight - 1)
                    result.AppendLine();
            }
            
            return result.ToString();
        }

        /// <summary>
        /// Create a FrameView containing the rendered map.
        /// FrameView is a Terminal.Gui view that draws a border around its content.
        /// </summary>
        public FrameView CreateMapView(MapModel mapModel, Rect bounds)
        {
            var mapView = new FrameView($"Map - {mapModel.GameConfig.GetLevelDisplayName(mapModel.CurrentLevel)}")
            {
                X = bounds.X,
                Y = bounds.Y,
                Width = bounds.Width,
                Height = bounds.Height
            };

            // Create a TextView to hold the ASCII map
            // TextView is a scrollable text display widget
            var mapTextView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true, // Player can't edit the map
                WordWrap = false, // Preserve ASCII art formatting
                Text = RenderMapToString(mapModel, bounds.Width - 4, bounds.Height - 4)
            };

            mapView.Add(mapTextView);
            return mapView;
        }

        /// <summary>
        /// Create a status view showing player location and progress.
        /// This creates a horizontal status bar with game information.
        /// </summary>
        public FrameView CreateStatusView(MapModel mapModel, string healthStatus, Rect bounds)
        {
            var statusView = new FrameView("Status")
            {
                X = bounds.X,
                Y = bounds.Y,
                Width = bounds.Width,
                Height = bounds.Height
            };

            var statusText = $"Location: {mapModel.GetCurrentRoomName()} | " +
                           $"Level: {mapModel.GameConfig.GetLevelDisplayName(mapModel.CurrentLevel)} | " +
                           $"Rooms Visited: {mapModel.VisitedRooms.Count} | " +
                           $"Health: {healthStatus}";

            var statusLabel = new Label(statusText)
            {
                X = 1,
                Y = 0,
                Width = Dim.Fill() - 1,
                Height = 1
            };

            statusView.Add(statusLabel);
            return statusView;
        }

        /// <summary>
        /// Create a legend view explaining map symbols.
        /// </summary>
        public FrameView CreateLegendView(MapModel mapModel, Rect bounds)
        {
            var legendView = new FrameView("Legend")
            {
                X = bounds.X,
                Y = bounds.Y,
                Width = bounds.Width,
                Height = bounds.Height
            };

            var legendText = "Map Symbols:\n" +
                           "@ = Your location\n" +
                           "+ = Items in room\n" +
                           ". = Horizontal path\n" +
                           ": = Vertical path\n" +
                           "^ = Stairs up\n" +
                           "v = Stairs down\n" +
                           "\n" +
                           "Colors:\n" +
                           "Green = Visited rooms\n" +
                           "Cyan = Current room\n" +
                           "White = Room with items";

            var legendTextView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                WordWrap = true,
                Text = legendText
            };

            legendView.Add(legendTextView);
            return legendView;
        }

        /// <summary>
        /// Update an existing map view with new data.
        /// This method allows for efficient updates without recreating the entire view.
        /// </summary>
        public void UpdateMapView(FrameView mapView, MapModel mapModel)
        {
            // Find the TextView within the FrameView
            var textView = mapView.Subviews.OfType<TextView>().FirstOrDefault();
            if (textView != null)
            {
                textView.Text = RenderMapToString(mapModel, 
                    mapView.Bounds.Width - 4, 
                    mapView.Bounds.Height - 4);
                textView.SetNeedsDisplay(); // Tell Terminal.Gui to redraw this view
            }

            // Update the title to reflect current level
            mapView.Title = $"Map - {mapModel.GameConfig.GetLevelDisplayName(mapModel.CurrentLevel)}";
            mapView.SetNeedsDisplay();
        }

        /// <summary>
        /// Update an existing status view with new data.
        /// </summary>
        public void UpdateStatusView(FrameView statusView, MapModel mapModel, string healthStatus)
        {
            var statusLabel = statusView.Subviews.OfType<Label>().FirstOrDefault();
            if (statusLabel != null)
            {
                var statusText = $"Location: {mapModel.GetCurrentRoomName()} | " +
                               $"Level: {mapModel.GameConfig.GetLevelDisplayName(mapModel.CurrentLevel)} | " +
                               $"Rooms Visited: {mapModel.VisitedRooms.Count} | " +
                               $"Health: {healthStatus}";
                
                statusLabel.Text = statusText;
                statusLabel.SetNeedsDisplay();
            }
        }

        /// <summary>
        /// Draw room boxes on the character map array.
        /// Each room is represented as a 4x3 character box.
        /// </summary>
        private void DrawRoomBoxOnMap(char[,] map, MapRoomData room, int minX, int minY, int mapWidth, int mapHeight)
        {
            var roomStartX = (room.X - minX) * 4;
            var roomStartY = (room.Y - minY) * 3;
            
            // Ensure we don't go out of bounds
            if (roomStartX + 3 >= mapWidth || roomStartY + 2 >= mapHeight || roomStartX < 0 || roomStartY < 0) 
                return;
            
            // Draw room box using box-drawing characters
            // Top border: +---+
            map[roomStartY, roomStartX] = '+';
            map[roomStartY, roomStartX + 1] = '-';
            map[roomStartY, roomStartX + 2] = '-';
            map[roomStartY, roomStartX + 3] = '+';
            
            // Middle row with room character and items indicator
            map[roomStartY + 1, roomStartX] = '|';
            
            if (room.IsCurrentRoom)
            {
                map[roomStartY + 1, roomStartX + 1] = '@'; // Player character
                map[roomStartY + 1, roomStartX + 2] = room.HasVisibleItems ? '+' : ' ';
            }
            else
            {
                map[roomStartY + 1, roomStartX + 1] = room.DisplayChar;
                map[roomStartY + 1, roomStartX + 2] = room.HasVisibleItems ? '+' : ' ';
            }
            
            map[roomStartY + 1, roomStartX + 3] = '|';
            
            // Bottom border: +---+
            map[roomStartY + 2, roomStartX] = '+';
            map[roomStartY + 2, roomStartX + 1] = '-';
            map[roomStartY + 2, roomStartX + 2] = '-';
            map[roomStartY + 2, roomStartX + 3] = '+';
        }

        /// <summary>
        /// Draw connection paths between rooms on the character map array.
        /// </summary>
        private void DrawConnectionsOnMap(char[,] map, List<MapRoomData> rooms, MapModel mapModel, 
            int minX, int minY, int mapWidth, int mapHeight)
        {
            foreach (var room in rooms)
            {
                var roomCenterX = (room.X - minX) * 4 + 2;
                var roomCenterY = (room.Y - minY) * 3 + 1;
                
                var connections = mapModel.GetConnectionsForRoom(room.RoomNumber);
                
                foreach (var connection in connections)
                {
                    DrawSingleConnection(map, connection, rooms, minX, minY, 
                        mapWidth, mapHeight, roomCenterX, roomCenterY);
                }
            }
        }

        /// <summary>
        /// Draw a single connection between two rooms.
        /// </summary>
        private void DrawSingleConnection(char[,] map, MapConnection connection, List<MapRoomData> rooms,
            int minX, int minY, int mapWidth, int mapHeight, int roomCenterX, int roomCenterY)
        {
            var targetRoom = rooms.FirstOrDefault(r => r.RoomNumber == connection.ToRoom);
            if (targetRoom == null) return;

            switch (connection.Direction.ToLower())
            {
                case "east":
                    var targetX = (targetRoom.X - minX) * 4;
                    for (int x = roomCenterX + 2; x < targetX && x < mapWidth; x++)
                    {
                        if (roomCenterY >= 0 && roomCenterY < mapHeight && map[roomCenterY, x] == ' ')
                            map[roomCenterY, x] = '.';
                    }
                    break;
                    
                case "south":
                    var targetY = (targetRoom.Y - minY) * 3;
                    for (int y = roomCenterY + 2; y < targetY && y < mapHeight; y++)
                    {
                        if (roomCenterX >= 0 && roomCenterX < mapWidth && map[y, roomCenterX] == ' ')
                            map[y, roomCenterX] = ':';
                    }
                    break;
                    
                case "up":
                    if (roomCenterX + 1 < mapWidth && roomCenterY - 1 >= 0 && map[roomCenterY - 1, roomCenterX + 1] == ' ')
                        map[roomCenterY - 1, roomCenterX + 1] = '^';
                    break;
                    
                case "down":
                    if (roomCenterX + 1 < mapWidth && roomCenterY + 3 < mapHeight && map[roomCenterY + 3, roomCenterX + 1] == ' ')
                        map[roomCenterY + 3, roomCenterX + 1] = 'v';
                    break;
            }
        }
    }
}