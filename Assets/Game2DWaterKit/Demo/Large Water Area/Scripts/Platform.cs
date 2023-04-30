namespace Game2DWaterKit.Demo
{
    using UnityEngine;

    public class Platform : MonoBehaviour
    {
        private struct BreakablePartJointInfo
        {
            public bool isBroken;
            public Vector2 connectedAnchor;
        }

        private BreakablePartJointInfo[] breakablePartsInfo;

        public FixedJoint2D[] breakableParts;

        private void Awake()
        {
            breakablePartsInfo = new BreakablePartJointInfo[breakableParts.Length];

            for (int i = 0, imax = breakableParts.Length; i < imax; i++)
            {
                breakableParts[i].autoConfigureConnectedAnchor = false;
                breakablePartsInfo[i].isBroken = false;
                breakablePartsInfo[i].connectedAnchor = breakableParts[i].connectedAnchor;
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            BreakRandomJoint();
        }

        public void BreakRandomJoint()
        {
            int index = Random.Range(0, breakableParts.Length);

            if (breakablePartsInfo[index].isBroken)
                return;

            breakableParts[index].enabled = false;
            breakablePartsInfo[index].isBroken = true;
        }

        public void ResetJoints()
        {
            for (int i = 0, imax = breakableParts.Length; i < imax; i++)
            {
                var joint = breakableParts[i];
                joint.connectedAnchor = breakablePartsInfo[i].connectedAnchor;
                joint.enabled = true;

                breakablePartsInfo[i].isBroken = false;
            }
        }
    }
}