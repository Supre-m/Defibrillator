namespace DesfribilatorPlugin
{
    using Exiled.API.Interfaces;

    public class Translation : ITranslation
    {
        public string MessageWhenYouRevive { get; private set; } = "You feel like you recover your life\n<color=#028EDC>{PlayerName} has revived you with a defibrillator!</color>";
        public string MessageWhenYouReviveSCP { get; private set; } = "<color=red>Someone make the error of reviving You...</color>\n<color=#028EDC>{PlayerName} has revived you with a defibrillator!</color>";

    }
}
