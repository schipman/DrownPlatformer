using Microsoft.Xna.Framework.Input;
using MonoFlixel;

namespace DrownPlatformer;

/// <summary>
/// The controls, as actions (keyboard or gamepad). Same keys as the original: arrows move and aim, X jumps, Z fires, C changes weapon,
/// TAB floats up for a moment. FlxG.Actions clears its sets when the state changes, so every play state makes a new one.
/// </summary>
public sealed class DrownInput
{
    public readonly FlxActionSet Set = new("drown");
    public readonly FlxActionAnalog Move;
    public readonly FlxActionDigital Jump, Fire, NextWeapon, Float, Confirm, Pause;

    public DrownInput()
    {
        Move = Set.Add(new FlxActionAnalog("move")
            .AddKeys(Keys.Up, Keys.Down, Keys.Left, Keys.Right)
            .AddGamepad(FlxGamepadInputID.LeftAnalogStick));
        Jump = Set.Add(new FlxActionDigital("jump").AddKey(Keys.X).AddGamepad(FlxGamepadInputID.A));
        Fire = Set.Add(new FlxActionDigital("fire").AddKey(Keys.Z).AddGamepad(FlxGamepadInputID.B));
        NextWeapon = Set.Add(new FlxActionDigital("weapon").AddKey(Keys.C).AddGamepad(FlxGamepadInputID.Y));
        Float = Set.Add(new FlxActionDigital("float").AddKey(Keys.Tab).AddGamepad(FlxGamepadInputID.X));
        Confirm = Set.Add(new FlxActionDigital("confirm").AddKey(Keys.Z).AddKey(Keys.Enter).AddGamepad(FlxGamepadInputID.A));
        Pause = Set.Add(new FlxActionDigital("pause").AddKey(Keys.P).AddKey(Keys.Escape).AddGamepad(FlxGamepadInputID.Start));
        FlxG.Actions.Activate(Set);
    }
}
