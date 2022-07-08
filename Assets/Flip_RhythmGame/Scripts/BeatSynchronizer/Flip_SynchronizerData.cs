// License
// The MIT License (MIT)
//
// Copyright (c) 2014 Christian Floisand
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Flip_SynchronizerData
{
    // BeatValue determines which beat to synchronize with, and is specified for each BeatCounter instance.
    // (A sequence of beat values are specified for PatternCounter instances).
    public enum BeatValue
    {
        None,
        SixteenthBeat,
        SixteenthDottedBeat,
        EighthBeat,
        EighthDottedBeat,
        QuarterBeat,
        QuarterDottedBeat,
        HalfBeat,
        HalfDottedBeat,
        WholeBeat,
        WholeDottedBeat
    }

    // BeatType is used to specify whether the beat value is an off-beat, on-beat, up-beat, or a down-beat.
    // This value is sent along with the notify message when a beat occurs so that different action
    // may be taken for the different beat types.
    // This information is stored in a beatMask field in each BeatObserver instance.
    public enum BeatType {
        None		= 0,
        OffBeat		= 1,
        OnBeat		= 2,
        UpBeat		= 4,
        DownBeat	= 8
    };

    // The decimal values associated with each beat value are used in calculating the sample position in the audio
    // where the beat will occur. The values array acts as a LUT, with index positions corresponding to BeatValue.
    // These values are relative to quarter beats (which have a value of 1).
    public struct BeatDecimalValues
    {
        private static float dottedBeatModifier = 1.5f;
        public static float[] values =
        {
            0f,
            4f, 4f/dottedBeatModifier,			// sixteenth, dotted sixteenth
            2f, 2f/dottedBeatModifier,			// eighth, dotted eighth
            1f, 1f/dottedBeatModifier,			// quarter, dotted quarter
            0.5f, 0.5f/dottedBeatModifier,		// half, dotted half
            0.25f, 0.25f/dottedBeatModifier		// whole, dotted whole
        };
    }
}
