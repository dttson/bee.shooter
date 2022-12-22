using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        public float distanceTravelled;

        public void reset()
        {
            if (pathCreator != null)
            {
                distanceTravelled = 0f;
                transform.position = pathCreator.path.GetPoint(0);
            }
        }

        public bool isReachTheEnd()
        {
            if (pathCreator == null) return false;
            var endPoint = pathCreator.path.GetPoint(pathCreator.path.NumPoints - 1);
            return Vector3.SqrMagnitude(transform.position - endPoint) < 0.0001f;
        }
        
        void Start() {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        void Update()
        {
            if (pathCreator != null)
            {
                distanceTravelled += speed * Time.deltaTime;
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                transform.Rotate(Vector3.right, -90f, Space.Self);
                transform.Rotate(Vector3.up, 90f, Space.Self);
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}