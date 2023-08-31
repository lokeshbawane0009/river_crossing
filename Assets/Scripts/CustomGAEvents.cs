using UnityEngine;
using static GameAnalyticsController;

public class CustomGAEvents : MonoBehaviour
{
    public static void HintButtonClickedGAEvent()
    {
        Miscellaneous.NewDesignEvent("Hint Used");
    }

    public static void LevelStart(int levelIndex)
    {
        LevelBasedProgressionRelated.LogLevelStartEvent(levelIndex);
    }

    public static void LevelFailed(int levelIndex)
    {
        LevelBasedProgressionRelated.LogLevelFailEvent(levelIndex);
    }

    public static void LevelCompleted(int levelIndex)
    {
        LevelBasedProgressionRelated.LogLevelEndEvent(levelIndex);
    }

    public static void LevelCompletedWithMoves(int levelIndex,int moves)
    {
        Miscellaneous.NewDesignEvent($"Level {levelIndex} Completed in",(float) moves);
    }
}
