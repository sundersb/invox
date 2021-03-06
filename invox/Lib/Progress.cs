﻿using System;

namespace invox.Lib {
    /// <summary>
    /// Console progress bar
    /// </summary>
    class Progress {
        const int LINE_LENGTH = 80;
        const int PROGRESS_WIDTH = LINE_LENGTH - 6;

        const ConsoleColor COLOR_TITLE = ConsoleColor.Yellow;
        const ConsoleColor COLOR_BAR = ConsoleColor.Green;

        int max;
        long step;
        long position;
        ConsoleColor fg;
        int left;
        int top;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="title">Progress title</param>
        /// <param name="maxPosition">Maximal position value</param>
        public Progress(string title, int maxPosition) {
            Console.CursorVisible = false;

            max = maxPosition;
            step = 0;
            position = 0;

            Console.WriteLine();

            top = Console.CursorTop;

            fg = Console.ForegroundColor;

            Console.ForegroundColor = COLOR_TITLE;
            Center(top, title + " (" + maxPosition.ToString() + ")");

            Console.ForegroundColor = fg;
            Console.Write(" [ ");

            left = Console.CursorLeft;
            top = Console.CursorTop;

            Console.SetCursorPosition(left + PROGRESS_WIDTH + 1, top);
            Console.Write(']');

            Console.SetCursorPosition(left, top);
            fg = Console.ForegroundColor;
            Center(top + 1, "0%");
        }

        /// <summary>
        /// Move progress position by one
        /// </summary>
        public void Step() {
            long newpos = ++position * PROGRESS_WIDTH / max;
            if (newpos > step) {
                step = newpos;
                int p = left + (int)step - 1;
                if (p < LINE_LENGTH) {
                    Console.ForegroundColor = COLOR_BAR;
                    Console.SetCursorPosition(p, top);
                    Console.Write('=');
                    Console.ForegroundColor = fg;
                }
                Center(top + 1, string.Format("{0}%-{1}", (long)position * 100 / max, position));
            }
        }

        void Center(int line, string text) {
            Console.SetCursorPosition((LINE_LENGTH - text.Length) / 2, top);
            Console.WriteLine(text);
        }

        /// <summary>
        /// Reset console to defaults after progress is over
        /// </summary>
        public void Close() {
            Console.CursorVisible = true;
            Console.WriteLine();
            Console.ForegroundColor = fg;
        }
    }
}
