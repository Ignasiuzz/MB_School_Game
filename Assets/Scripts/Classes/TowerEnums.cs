public enum DefenseState
{
    Inactive,
    Preparing,
    WaveInProgress,
    Won,
    Lost
}

public enum TargetMode
{
    First,      // furthest along the path
    Closest,
    Strongest,  // most health
    Weakest     // least health
}
