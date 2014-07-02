/*
 * Copyright 2014 MovingBlocks
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Assets.BresenhamLine
{
    /**
     * Based on code from the Terasology projects by Martin Steiger
     * https://github.com/Terasology/Cities/blob/master/src/test/java/org/terasology/cities/swing/draw/BresenhamLine.java
     */

    public class BresenhamLine
    {
        private int edgeLength;
        private byte[] canvas;

        public BresenhamLine(byte[] canvas, int edgeLength)
        {
            this.canvas = canvas;
            this.edgeLength = edgeLength;
        }

        private void DrawPixel(int x, int y)
        {
            canvas[x + edgeLength * y] = 0;
        }

        /**
         * Bresenham line with a thickness of 1
         * @param xStart x0
         * @param yStart y0
         * @param xEnd x1
         * @param yEnd y1
         */

        public void DrawLine(int xStart, int yStart, int xEnd, int yEnd)
        {
            DrawLineOverlap(xStart, yStart, xEnd, yEnd, Overlap.NONE);
        }

        /*
         * modified Bresenham with optional overlap (esp. for drawThickLine())
         * Overlap draws additional pixel when changing minor direction - for standard bresenham overlap = LINE_OVERLAP_NONE (0)
         * <pre>
         *  Sample line:
         *    00+
         *     -0000+
         *         -0000+
         *             -00
         *  0 pixels are drawn for normal line without any overlap
         *  + pixels are drawn if LINE_OVERLAP_MAJOR
         *  - pixels are drawn if LINE_OVERLAP_MINOR
         * </pre>
         */

        private void DrawLineOverlap(int xStart, int yStart, int xEnd, int yEnd, Overlap aOverlap)
        {
            int tDeltaX;
            int tDeltaY;
            int tDeltaXTimes2;
            int tDeltaYTimes2;
            int tError;
            int tStepX;
            int tStepY;

            int aXStart = xStart;
            int aYStart = yStart;

            /*
             * Clip to display size
             */

            if (aXStart >= edgeLength)
            {
                aXStart = edgeLength - 1;
            }
            if (aXStart < 0)
            {
                aXStart = 0;
            }
            if (xEnd >= edgeLength)
            {
                xEnd = edgeLength - 1;
            }
            if (xEnd < 0)
            {
                xEnd = 0;
            }
            if (aYStart >= edgeLength)
            {
                aYStart = edgeLength - 1;
            }
            if (aYStart < 0)
            {
                aYStart = 0;
            }
            if (yEnd >= edgeLength)
            {
                yEnd = edgeLength - 1;
            }
            if (yEnd < 0)
            {
                yEnd = 0;
            }

            if ((aXStart == xEnd) || (aYStart == yEnd))
            {
                //horizontal or vertical line -> fillRect() is faster
                DrawFilledRect(aXStart, aYStart, xEnd, yEnd);
            }
            else
            {
                //calculate direction
                tDeltaX = xEnd - aXStart;
                tDeltaY = yEnd - aYStart;
                if (tDeltaX < 0)
                {
                    tDeltaX = -tDeltaX;
                    tStepX = -1;
                }
                else
                {
                    tStepX = +1;
                }
                if (tDeltaY < 0)
                {
                    tDeltaY = -tDeltaY;
                    tStepY = -1;
                }
                else
                {
                    tStepY = +1;
                }
                tDeltaXTimes2 = tDeltaX << 1;
                tDeltaYTimes2 = tDeltaY << 1;
                //draw start pixel
                DrawPixel(aXStart, aYStart);
                if (tDeltaX > tDeltaY)
                {
                    // start value represents a half step in Y direction
                    tError = tDeltaYTimes2 - tDeltaX;
                    while (aXStart != xEnd)
                    {
                        // step in main direction
                        aXStart += tStepX;
                        if (tError >= 0)
                        {
                            if ((aOverlap & Overlap.MAJOR) == Overlap.MAJOR)
                            {
                                // draw pixel in main direction before changing
                                DrawPixel(aXStart, aYStart);
                            }
                            // change Y
                            aYStart += tStepY;
                            if ((aOverlap & Overlap.MINOR) == Overlap.MINOR)
                            {
                                // draw pixel in minor direction before changing
                                DrawPixel(aXStart - tStepX, aYStart);
                            }
                            tError -= tDeltaXTimes2;
                        }
                        tError += tDeltaYTimes2;
                        DrawPixel(aXStart, aYStart);
                    }
                }
                else
                {
                    tError = tDeltaXTimes2 - tDeltaY;
                    while (aYStart != yEnd)
                    {
                        aYStart += tStepY;
                        if (tError >= 0)
                        {
                            if ((aOverlap & Overlap.MAJOR) == Overlap.MAJOR)
                            {
                                // draw pixel in main direction before changing
                                DrawPixel(aXStart, aYStart);
                            }
                            aXStart += tStepX;
                            if ((aOverlap & Overlap.MINOR) == Overlap.MINOR)
                            {
                                // draw pixel in minor direction before changing
                                DrawPixel(aXStart, aYStart - tStepY);
                            }
                            tError -= tDeltaYTimes2;
                        }
                        tError += tDeltaXTimes2;
                        DrawPixel(aXStart, aYStart);
                    }
                }
            }
        }

        private void DrawFilledRect(int x0, int y0, int x1, int y1)
        {
            int sx = System.Math.Min(x0, x1);
            int sy = System.Math.Min(y0, y1);
            int ex = System.Math.Max(x0, x1);
            int ey = System.Math.Max(y0, y1);

            for (int y = sy; y <= ey; y++)
            {
                for (int x = sx; x <= ex; x++)
                {
                    DrawPixel(x, y);
                }
            }
        }

        /**
         * Bresenham with thickness - no pixel missed and every pixel only drawn once!
         * @param x0 x0
         * @param y0 y0
         * @param x1 x1
         * @param y1 y1
         * @param thickness the thickness
         * @param mode the mode
         */

        public void DrawThickLine(int x0, int y0, int x1, int y1, int thickness, ThicknessMode mode)
        {
            if (thickness <= 1)
            {
                DrawLineOverlap(x0, y0, x1, y1, Overlap.NONE);
            }

            int tDeltaX;
            int tDeltaY;
            int tDeltaXTimes2;
            int tDeltaYTimes2;
            int tError;
            int tStepX;
            int tStepY;

            int aXStart = x0;
            int aYStart = y0;
            int aXEnd = x1;
            int aYEnd = y1;

            /**
             * For coordinatesystem with 0.0 topleft
             * Swap X and Y delta and calculate clockwise (new delta X inverted)
             * or counterclockwise (new delta Y inverted) rectangular direction.
             * The right rectangular direction for LineOverlap.MAJOR toggles with each octant
             */
            tDeltaY = aXEnd - aXStart;
            tDeltaX = aYEnd - aYStart;
            // mirror 4 quadrants to one and adjust deltas and stepping direction
            bool tSwap = true; // count effective mirroring
            if (tDeltaX < 0)
            {
                tDeltaX = -tDeltaX;
                tStepX = -1;
                tSwap = !tSwap;
            }
            else
            {
                tStepX = +1;
            }
            if (tDeltaY < 0)
            {
                tDeltaY = -tDeltaY;
                tStepY = -1;
                tSwap = !tSwap;
            }
            else
            {
                tStepY = +1;
            }
            tDeltaXTimes2 = tDeltaX << 1;
            tDeltaYTimes2 = tDeltaY << 1;
            Overlap tOverlap;

            // adjust for right direction of thickness from line origin
            int tDrawStartAdjustCount;

            switch (mode)
            {
                case ThicknessMode.COUNTERCLOCKWISE:
                    tDrawStartAdjustCount = thickness - 1;
                    break;

                case ThicknessMode.CLOCKWISE:
                    tDrawStartAdjustCount = 0;
                    break;

                case ThicknessMode.MIDDLE:
                    tDrawStartAdjustCount = thickness / 2;
                    break;

                default:
                    throw new System.InvalidOperationException();
            }

            // which octant are we now
            if (tDeltaX >= tDeltaY)
            {
                if (tSwap)
                {
                    tDrawStartAdjustCount = (thickness - 1) - tDrawStartAdjustCount;
                    tStepY = -tStepY;
                }
                else
                {
                    tStepX = -tStepX;
                }
                /*
                 * Vector for draw direction of lines is rectangular and counterclockwise to original line
                 * Therefore no pixel will be missed if LINE_OVERLAP_MAJOR is used
                 * on changing in minor rectangular direction
                 */
                // adjust draw start point
                tError = tDeltaYTimes2 - tDeltaX;
                for (int i = tDrawStartAdjustCount; i > 0; i--)
                {
                    // change X (main direction here)
                    aXStart -= tStepX;
                    aXEnd -= tStepX;
                    if (tError >= 0)
                    {
                        // change Y
                        aYStart -= tStepY;
                        aYEnd -= tStepY;
                        tError -= tDeltaXTimes2;
                    }
                    tError += tDeltaYTimes2;
                }
                //draw start line
                DrawLine(aXStart, aYStart, aXEnd, aYEnd);
                // draw aThickness lines
                tError = tDeltaYTimes2 - tDeltaX;
                for (int i = thickness; i > 1; i--)
                {
                    // change X (main direction here)
                    aXStart += tStepX;
                    aXEnd += tStepX;
                    tOverlap = Overlap.NONE;
                    if (tError >= 0)
                    {
                        // change Y
                        aYStart += tStepY;
                        aYEnd += tStepY;
                        tError -= tDeltaXTimes2;
                        /*
                         * change in minor direction reverse to line (main) direction
                         * because of chosing the right (counter)clockwise draw vector
                         * use LINE_OVERLAP_MAJOR to fill all pixel
                         *
                         * EXAMPLE:
                         * 1,2 = Pixel of first lines
                         * 3 = Pixel of third line in normal line mode
                         * - = Pixel which will be drawn in LINE_OVERLAP_MAJOR mode
                         *           33
                         *       3333-22
                         *   3333-222211
                         * 33-22221111
                         *  221111                     /\
                             *  11                          Main direction of draw vector
                         *  -> Line main direction
                         *  <- Minor direction of counterclockwise draw vector
                         */
                        tOverlap = Overlap.MAJOR;
                    }
                    tError += tDeltaYTimes2;
                    DrawLineOverlap(aXStart, aYStart, aXEnd, aYEnd, tOverlap);
                }
            }
            else
            {
                // the other octant
                if (tSwap)
                {
                    tStepX = -tStepX;
                }
                else
                {
                    tDrawStartAdjustCount = (thickness - 1) - tDrawStartAdjustCount;
                    tStepY = -tStepY;
                }
                // adjust draw start point
                tError = tDeltaXTimes2 - tDeltaY;
                for (int i = tDrawStartAdjustCount; i > 0; i--)
                {
                    aYStart -= tStepY;
                    aYEnd -= tStepY;
                    if (tError >= 0)
                    {
                        aXStart -= tStepX;
                        aXEnd -= tStepX;
                        tError -= tDeltaYTimes2;
                    }
                    tError += tDeltaXTimes2;
                }
                //draw start line
                DrawLine(aXStart, aYStart, aXEnd, aYEnd);
                tError = tDeltaXTimes2 - tDeltaY;
                for (int i = thickness; i > 1; i--)
                {
                    aYStart += tStepY;
                    aYEnd += tStepY;
                    tOverlap = Overlap.NONE;
                    if (tError >= 0)
                    {
                        aXStart += tStepX;
                        aXEnd += tStepX;
                        tError -= tDeltaYTimes2;
                        tOverlap = Overlap.MAJOR;
                    }
                    tError += tDeltaXTimes2;
                    DrawLineOverlap(aXStart, aYStart, aXEnd, aYEnd, tOverlap);
                }
            }
        }

        /**
         * Bresenham with thickness, but no clipping, some pixel are drawn twice (use LINE_OVERLAP_BOTH)
         * and direction of thickness changes for each octant (except for LINE_THICKNESS_MIDDLE and aThickness odd)
         * @param x0 x0
         * @param y0 y0
         * @param x1 x1
         * @param y1 y1
         * @param thickness the thickness
         * @param mode the mode
         */

        public void DrawThickLineSimple(int x0, int y0, int x1, int y1, int thickness, ThicknessMode mode)
        {
            int tDeltaX;
            int tDeltaY;
            int tDeltaXTimes2;
            int tDeltaYTimes2;
            int tError;
            int tStepX;
            int tStepY;

            int aXStart = x0;
            int aYStart = y0;
            int aXEnd = x1;
            int aYEnd = y1;

            tDeltaY = aXStart - aXEnd;
            tDeltaX = aYEnd - aYStart;
            // mirror 4 quadrants to one and adjust deltas and stepping direction
            if (tDeltaX < 0)
            {
                tDeltaX = -tDeltaX;
                tStepX = -1;
            }
            else
            {
                tStepX = +1;
            }
            if (tDeltaY < 0)
            {
                tDeltaY = -tDeltaY;
                tStepY = -1;
            }
            else
            {
                tStepY = +1;
            }
            tDeltaXTimes2 = tDeltaX << 1;
            tDeltaYTimes2 = tDeltaY << 1;
            Overlap tOverlap;
            // which octant are we now
            if (tDeltaX > tDeltaY)
            {
                if (mode == ThicknessMode.MIDDLE)
                {
                    // adjust draw start point
                    tError = tDeltaYTimes2 - tDeltaX;
                    for (int i = thickness / 2; i > 0; i--)
                    {
                        // change X (main direction here)
                        aXStart -= tStepX;
                        aXEnd -= tStepX;
                        if (tError >= 0)
                        {
                            // change Y
                            aYStart -= tStepY;
                            aYEnd -= tStepY;
                            tError -= tDeltaXTimes2;
                        }
                        tError += tDeltaYTimes2;
                    }
                }
                //draw start line
                DrawLine(aXStart, aYStart, aXEnd, aYEnd);
                // draw aThickness lines
                tError = tDeltaYTimes2 - tDeltaX;
                for (int i = thickness; i > 1; i--)
                {
                    // change X (main direction here)
                    aXStart += tStepX;
                    aXEnd += tStepX;
                    tOverlap = Overlap.NONE;
                    if (tError >= 0)
                    {
                        // change Y
                        aYStart += tStepY;
                        aYEnd += tStepY;
                        tError -= tDeltaXTimes2;
                        tOverlap = (Overlap.MINOR | Overlap.MAJOR);
                    }
                    tError += tDeltaYTimes2;
                    DrawLineOverlap(aXStart, aYStart, aXEnd, aYEnd, tOverlap);
                }
            }
            else
            {
                // adjust draw start point
                if (mode == ThicknessMode.MIDDLE)
                {
                    tError = tDeltaXTimes2 - tDeltaY;
                    for (int i = thickness / 2; i > 0; i--)
                    {
                        aYStart -= tStepY;
                        aYEnd -= tStepY;
                        if (tError >= 0)
                        {
                            aXStart -= tStepX;
                            aXEnd -= tStepX;
                            tError -= tDeltaYTimes2;
                        }
                        tError += tDeltaXTimes2;
                    }
                }
                //draw start line
                DrawLine(aXStart, aYStart, aXEnd, aYEnd);
                tError = tDeltaXTimes2 - tDeltaY;
                for (int i = thickness; i > 1; i--)
                {
                    aYStart += tStepY;
                    aYEnd += tStepY;
                    tOverlap = Overlap.NONE;
                    if (tError >= 0)
                    {
                        aXStart += tStepX;
                        aXEnd += tStepX;
                        tError -= tDeltaYTimes2;
                        tOverlap = (Overlap.MINOR | Overlap.MAJOR);
                    }
                    tError += tDeltaXTimes2;
                    DrawLineOverlap(aXStart, aYStart, aXEnd, aYEnd, tOverlap);
                }
            }
        }
    }
}