using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Commands;
using StorybrewCommon.Storyboarding.CommandValues;
using StorybrewCommon.Storyboarding.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Util
{
    // contains move commands that can be adjusted and later applied to a sprite
    public class Movement
    {
        private List<MoveCommand> commands = new List<MoveCommand>();

        private bool newCommandsAdded;

        public IEnumerable<MoveCommand> Commands
        {
            get
            {
                if (newCommandsAdded)
                {
                    newCommandsAdded = false;
                    return commands = commands.OrderBy(_ => _.StartTime).ToList();
                }
                else return commands;
            }
        }

        public Movement() {}

        public Movement(Movement other)
        {
            commands = other.Commands.ToList();
        }

        public Movement(OsbEasing easing, double startTime, double endTime, Vector2 startPosition, Vector2 endPosition)
        {
            AddCommand(easing, startTime, endTime, startPosition, endPosition);
        }

        public Movement AddCommand(OsbEasing easing, double startTime, double endTime, Vector2 startPosition, Vector2 endPosition)
        {
            commands.Add(new MoveCommand(easing, startTime, endTime, startPosition, endPosition));
            newCommandsAdded = true;
            return this;
        }

        public Movement AddCommand(MoveCommand cmd)
        {
            commands.Add(cmd);
            newCommandsAdded = true;
            return this;
        }

        // offset in space
        public static Movement operator +(Movement self, Vector2 offset)
        {
            var result = new Movement();
            foreach (var cmd in self.commands)
                result.AddCommand(new MoveCommand(
                    cmd.Easing,
                    cmd.StartTime,
                    cmd.EndTime,
                    (Vector2) cmd.StartValue + offset,
                    (Vector2) cmd.EndValue + offset
                ));
            return result;
        }

        public static Movement operator -(Movement self, Vector2 offset)
            => self + -offset;


        // offset in time
        public static Movement operator +(Movement self, double offset)
        {
            var result = new Movement();
            foreach (var cmd in self.commands)
                result.AddCommand(new MoveCommand(
                    cmd.Easing,
                    cmd.StartTime + offset,
                    cmd.EndTime + offset,
                    cmd.StartValue,
                    cmd.EndValue
                ));
            return result;
        }

        public static Movement operator -(Movement self, double offset)
            => self + -offset;

        public Movement Stretched(double factor, double fixedPoint)
        {
            var result = new Movement();
            foreach (var cmd in Commands)
                result.AddCommand(new MoveCommand(
                    cmd.Easing,
                    (cmd.StartTime - fixedPoint) * factor + fixedPoint,
                    (cmd.EndTime - fixedPoint) * factor + fixedPoint,
                    cmd.StartValue,
                    cmd.EndValue
                ));
            return result;
        }

        public Movement Stretched(double factor)
            => Stretched(factor, StartTime);


        private static MoveCommand addToEndValue(MoveCommand command, Vector2 vector)
            => new MoveCommand(
                command.Easing,
                command.StartTime,
                command.EndTime,
                command.StartValue,
                command.EndValue + (CommandPosition) vector
            );

        private static MoveCommand translate(MoveCommand command, Vector2 vector)
            => new MoveCommand(
                command.Easing,
                command.StartTime,
                command.EndTime,
                command.StartValue + (CommandPosition) vector,
                command.EndValue + (CommandPosition) vector
            );

        private static MoveCommand rotate(MoveCommand command, double angle, Vector2 origin)
            => new MoveCommand(
                command.Easing,
                command.StartTime,
                command.EndTime,
                ((Vector2) command.StartValue).rotated(angle, origin),
                ((Vector2) command.EndValue).rotated(angle, origin)
            );

        public Movement Relative()
        {
            Movement result = new Movement();
            foreach (var command in Commands)
                result.AddCommand(translate(command, -((Vector2) command.StartValue)));
            return result;
        }

        public static Movement Combined(Movement self, Movement other, double delta = 1000.0 / 60)
        {
            Movement result = new Movement();
            Movement resampled = self.ResampledSection(self.StartTime, self.EndTime, delta);
            Movement relativeMovement = other.Relative();
            foreach (var command in resampled.Commands)
                result.AddCommand(addToEndValue(command, relativeMovement.PositionAtTime(command.EndTime)));
            return result;
        }

        public static Movement operator *(Movement self, Movement other)
            => Combined(self, other);

        public Movement Rotated(double angle, Vector2 origin)
        {
            Movement result = new Movement();
            foreach (var command in Commands)
                result.AddCommand(rotate(command, angle, origin));
            return result;
        }

        public Vector2 StartPosition
        {
            get
            {
                if (!Commands.Any())
                    throw new InvalidOperationException("Movement cannot be empty before accessing its StartPosition.");
                return Commands.First().StartValue;
            }
        }

        public Vector2 EndPosition
        {
            get
            {
                if (!Commands.Any())
                    throw new InvalidOperationException("Movement cannot be empty before accessing its EndPosition.");
                return Commands.Last().EndValue;
            }
        }

        public double StartTime
        {
            get
            {
                if (!Commands.Any())
                    throw new InvalidOperationException("Movement cannot be empty before accessing its StartTime.");
                return Commands.First().StartTime;
            }
        }

        public double EndTime
        {
            get
            {
                if (!Commands.Any())
                    throw new InvalidOperationException("Movement cannot be empty before accessing its EndTime.");
                return Commands.Last().EndTime;
            }
        }

        public Vector2 PositionAtTime(double time)
        {
            if (!Commands.Any()) return OsbSprite.DefaultPosition;
            MoveCommand previous = null;
            foreach (var cmd in Commands)
            {
                if (cmd.EndTime > time)
                    if (cmd.StartTime <= time)
                        return cmd.ValueAtTime(time);
                    else
                        return previous?.EndValue ?? cmd.StartValue;
                previous = cmd;
            }
            return Commands.Last().EndValue;
        }

        // useful if part of an easing needs to be matched
        public Movement ResampledSection(double startTime, double endTime, double timeBetweenCommands)
        {
            var movement = new Movement();
            for (double t = startTime; t < endTime; t += timeBetweenCommands)
            {
                double t2 = t + timeBetweenCommands;
                if (t2 > endTime) t2 = endTime;
                movement.AddCommand(OsbEasing.None, t, t2, PositionAtTime(t), PositionAtTime(t2));
            }
            return movement;
        }

        public void ApplyTo(OsbSprite sprite)
        {
            foreach (var c in commands)
                sprite.Move(c.Easing, c.StartTime, c.EndTime, c.StartValue, c.EndValue);
        }
    }
}