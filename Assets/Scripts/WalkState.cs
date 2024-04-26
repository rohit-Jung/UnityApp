using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : StateMachineBehaviour
{
    float timer;
    NavMeshAgent agent;

    // Adjust the speed of rotation to make it smoother
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float timeForWalk = 7f;

    // Store the default angular speed
    private float defaultAngularSpeed;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        agent = animator.GetComponent<NavMeshAgent>();
        Vector3 randomPoint = GetRandomPointOnNavMesh(20f); // Get a random point within 20 units
        agent.SetDestination(randomPoint);

        // Store the default angular speed
        defaultAngularSpeed = agent.angularSpeed;

        // Set the agent's angular speed to make rotation smoother
        agent.angularSpeed = rotationSpeed;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 randomPoint = GetRandomPointOnNavMesh(20f); // Get a random point within 20 units
            agent.SetDestination(randomPoint);
        }
        timer += Time.deltaTime;
        if (timer > timeForWalk)
        {
            animator.SetBool("isWalking", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position);

        // Reset the agent's angular speed to its default value
        agent.angularSpeed = defaultAngularSpeed;
    }

    // Function to generate a random point on the NavMesh
    private Vector3 GetRandomPointOnNavMesh(float maxDistance)
    {
        Vector3 randomPosition = Vector3.zero;
        Vector3 randomDirection = Random.insideUnitSphere * maxDistance;
        randomDirection += agent.transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, maxDistance, NavMesh.AllAreas))
        {
            randomPosition = hit.position;
        }

        return randomPosition;
    }



    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}