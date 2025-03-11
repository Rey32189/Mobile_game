using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartPoint
{
    public static class GlobalSettings
    {
        #region Constants
        public static float CheckpointRadius = 0.1f;
        public static float DirectionHandleRadius = 0.6f;
        public static float LabelRenderDistance = 4.5f;
        public static float indentField = 25f;
        public static float boolIndentField = 6f;
        #endregion

        #region Colors
        public static Color SelectedActiveColor { get; private set; } = Color.green;
        public static Color SelectedInactiveColor { get; private set; } = Color.red;
        public static Color ActiveColor { get; private set; } = new Color(0.2f, 1, 0.2f, 1f);
        public static Color InactiveColor { get; private set; } = new Color(1, 0.2f, 0.2f, 1f);
        public static Color WireFrameColor { get; private set; } = new Color(0.7f, 1f, 0.4f);
        public static Color DirectionLineColor { get; private set; } = new Color(0f, 0f, 0.7f);
        public static Color LabelColor { get; private set; } = Color.black;
        public static Color DirectionCircleColor { get; private set; } = Color.white;
        public static Color SphereColor { get; private set; } = new Color(0.7f, 1f, 0.4f);
        public static Color InnerRadiusColor { get; private set; } = Color.red;
        public static Color OuterRadiusColor { get; private set; } = Color.green;
        public static Color GrayedOutColor { get; private set; } = Color.gray;
        public static Color GrayedOutFaceColor { get; private set; } = new Color(0.5f, 0.5f, 0.5f, 0.1f);
        public static Color RectangleFaceColor { get; private set; } = new Color(0, 1f, 0, 0.1f);
        public static Color RectangleOutlineColor { get; private set; } = Color.green;
        public static Color PrismColor { get; private set; } = new Color(0.7f, 1f, 0.4f);
        #endregion

    }

    #region Enums
    //Enum to determine how checkpoints will activate
    public enum ActivationMode
    {
        ManualActivation,
        AlwaysActive,
        ActivateOnCollision,
        Proximity
    }
    //Enum to determine what entities checkpoints act on.
    public enum EntityMode
    {
        SingleEntity,
        MultipleEntity,
        TagMode
    }
    //Enum to specify conditions that might block a checkpoint from being activated. See tooltip or documentation for more info.
    public enum ActivationOrder
    {
        None,
        Sequential
    }
    //Enum to specify what checkpoints OnCollision triggers on.
    public enum ColliderMode
    {
        None,
        ActiveCheckpoints,
        InactiveCheckpoints,
        Both
    }
    public enum TeleportMode
    {
        Nearest,
        HighestIndex,
        MostRecentlyActivated,
        Random
    }
    //Spawner enums
    public enum SpawnMode
    {
        Manual,
        SingleBurst,
        Constant,
        Refill
    }
    public enum SpawnArea
    {
        Point,
        Circle,
        Rect,
        Sphere,
        RectPrism,
        UseCheckpointCollider
    }
    public enum SpawnLocation
    {
        HighestIndex,
        MostRecentlyActivated,
        NearestToEntity,
        Random
    }
    public enum SpawnDirection
    {
        Custom,
        FaceEntity,
        AlignWithCheckpoint,
        Random
    }
    public enum ExitCondition
    {
        None,
        Time,
        SpawnCount
    }
    #endregion
}
