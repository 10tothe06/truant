using UnityEngine;

[System.Serializable]
public class player_keypresspacket
{
    public bool forward; // w
    public bool left; // a
    public bool back; // s
    public bool right; // d

    public bool jump; // space

    public bool crouch; // ctrl, usually
    public bool sprint; // shift

    public float horizontalMouse;
    public float verticalMouse;

    public bool mouseLeft;
    public bool mouseRight;

    public bool up; // e
    public bool down; // q

    public player_keypresspacket() {}

    public float GetHorizontal()
    {
        return (left ? -1 : 0) + (right ? 1 : 0);
    }

    public float GetVertical()
    {
        return (back ? -1 : 0) + (forward ? 1 : 0);
    }

    public string ParseToString()
    {
        string result = "";

        result += (forward ? "1" : "0") + ',';
       
        result += (back ? "1" : "0") + ',';
        result += (left ? "1" : "0") + ',';
        result += (right ? "1" : "0") + ',';

        result += (jump ? "1" : "0") + ',';

        result += (crouch ? "1" : "0") + ',';
        result += (sprint ? "1" : "0") + ',';

        result += horizontalMouse.ToString() + ',';
        result += verticalMouse.ToString() + ',';

        result += (up ? "1" : "0") + ',';
        result += (down ? "1" : "0") + ',';

        result += (mouseLeft ? "1" : "0") + ',';
        result += (mouseRight ? "1" : "0") + ',';

        return result;
    }

    public static player_keypresspacket ParseFromString(string s)
    {
        player_keypresspacket result = new player_keypresspacket();

        string[] split = util_string.SplitByChar(s,',');
        //Debug.Log(s);

        result.forward = split[0] == "1";
        result.back = split[1] == "1";
        result.left = split[2] == "1";
        result.right = split[3] == "1";

        result.jump = split[4] == "1";

        result.crouch = split[5] == "1";
        result.sprint = split[6] == "1";

        result.horizontalMouse = float.Parse(split[7]);
        result.verticalMouse = float.Parse(split[8]);

        result.up = split[9] == "1";
        result.down = split[10] == "1";

        result.mouseLeft = split[11] == "1";
        result.mouseRight = split[12] == "1";

        return result;
    }
}
