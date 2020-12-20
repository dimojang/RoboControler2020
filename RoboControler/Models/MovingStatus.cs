using System;
using System.Collections.Generic;
using System.Text;

namespace RoboControler.Models
{
    class MovingStatus
    {
        public Orientation orientation { get; set; } = Orientation.Stop;
        public int speed { get; set; } = 0;
        public int rotation { get; set; } = 0;

        public enum Orientation
        {
            Front = 0,
            Back = 1,
            Stop = -1
        }

        public bool Equals(MovingStatus movingStatus)
        {
            bool result = true;

            if (movingStatus.orientation != orientation) result = false;
            if (movingStatus.rotation != rotation) result = false;
            if (movingStatus.speed != speed) result = false;

            return result;
        }
    }
}
