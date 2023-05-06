using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class AISetter : MonoBehaviour
{
    [SerializeField] private float chaseRadius = 5f;
    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObject = PlayerController.Instance.gameObject;
        AIDestinationSetter aiDestinationSetter = GetComponent<AIDestinationSetter>();
        aiDestinationSetter.target = playerObject.transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
        if (distanceToPlayer <= chaseRadius && BattleSceneTransition.battleActive == false)
        {
            GetComponent<AIPath>().enabled = true;
        }
        else if (distanceToPlayer > chaseRadius || BattleSceneTransition.battleActive == true)
        {
            GetComponent<AIPath>().enabled = false;
        }
    }

}
