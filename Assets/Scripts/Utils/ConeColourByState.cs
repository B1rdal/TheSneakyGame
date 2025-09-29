using UnityEngine;

/// <summary>
/// Tints the vision cone mesh based on the guard's current FSM state.
/// Uses MaterialPropertyBlock to avoid instantiating materials at runtime.
/// 
/// Hook this up by assigning:
/// - 'guardAI' to your GuardSimpleAI (or whatever holds the FSM)
/// - 'coneRenderer' to the MeshRenderer on the VisionCone child
/// - Choose colors per state in the Inspector
/// </summary>
public class ConeColorByState : MonoBehaviour
{
    [Header("Refs")]
    public Guard guardAI;          // your AI MonoBehaviour with fsm.Current
    public MeshRenderer coneRenderer;      // MeshRenderer on the VisionCone child

    [Header("Colors")]
    public Color patrolColor = new Color(0f, 1f, 0.2f, 0.3f);  // green-ish, 30% alpha
    public Color chaseColor = new Color(1f, 0.2f, 0.1f, 0.35f); // red-ish
    public Color attackColor = new Color(1f, 0.6f, 0.1f, 0.4f);  // orange
    public Color returnColor = new Color(0.2f, 0.6f, 1f, 0.3f);  // blue

    // Shader color property IDs. Standard uses "_Color"; URP Lit uses "_BaseColor".
    static readonly int ColorID_Standard = Shader.PropertyToID("_Color");
    static readonly int ColorID_URP = Shader.PropertyToID("_BaseColor");

    MaterialPropertyBlock mpb;
    string lastStateName;

    void Awake()
    {
        if (!coneRenderer) coneRenderer = GetComponent<MeshRenderer>();
        mpb = new MaterialPropertyBlock();
        // Initialize color immediately
        ApplyColorForState(GetStateName());
        lastStateName = GetStateName();
    }

    void Update()
    {
        string stateName = GetStateName();
        if (stateName != lastStateName)
        {
            ApplyColorForState(stateName);
            lastStateName = stateName;
        }
    }

    string GetStateName()
    {
        // Fallback-safe: guardAI could be null during setup
        return guardAI && guardAI.FsmCurrentName != null ? guardAI.FsmCurrentName : "Patrol";
    }

    void ApplyColorForState(string stateName)
    {
        // Pick a color based on the state's display name
        Color c = patrolColor;
        switch (stateName)
        {
            case "Chase": c = chaseColor; break;
            case "Attack": c = attackColor; break;
            case "Return": c = returnColor; break;
                // default: Patrol/Idle/etc. -> patrolColor
        }

        // Read-modify-write MPB so we don't clear other properties by accident
        coneRenderer.GetPropertyBlock(mpb);

        // Try URP first, then Standard
        var mat = coneRenderer.sharedMaterial;
        if (mat != null && mat.HasProperty("_BaseColor"))
            mpb.SetColor(ColorID_URP, c);
        else
            mpb.SetColor(ColorID_Standard, c);

        coneRenderer.SetPropertyBlock(mpb);
    }
}
