using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ARRaycastManager))]
public class Indicador : MonoBehaviour
{
    [SerializeField]
    GameObject indicador;

    [SerializeField]
    GameObject baseTile;

    [SerializeField]
    GameObject stackTile;

    GameObject stacked;


    [SerializeField]
    InputAction touchInput;

    private BoxCollider baseCollider;
    private BoxCollider tileCollider;

    private float errorMargin = 0.01f;


    ARRaycastManager aRRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();

        touchInput.performed += _ => { TouchListener(); };

        indicador.SetActive(false);
        baseTile.SetActive(false);
        stackTile.SetActive(false);
    }

    private void OnEnable()
    {
        touchInput.Enable();
    }

    private void OnDisable()
    {
        touchInput.Disable();
    }


    void Update()
    {

        if (aRRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon) && !baseTile.activeSelf)
        {
            var hitPose = hits[0].pose;
            indicador.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);

            Vector3 lookPos = Camera.main.transform.position - indicador.transform.position;
            lookPos.y = 0;
            indicador.transform.rotation = Quaternion.LookRotation(lookPos);

            if (!indicador.activeInHierarchy)
            {
                indicador.SetActive(true);
            }
        }
        else
        {
            indicador.SetActive(false);
        }
    }


    private void TouchListener()
    {
        if (!baseTile.activeInHierarchy)
        {
            PlaceBaseTile();
        }
        else
        {
            bool entrouMargin = false;
            stackTile.GetComponent<MoveTile>().enabled = false;

            if (tileCollider.bounds.max.x < baseCollider.bounds.min.x ||
                tileCollider.bounds.min.x > baseCollider.bounds.max.x ||
                tileCollider.bounds.max.z < baseCollider.bounds.min.z ||
                tileCollider.bounds.min.z > baseCollider.bounds.max.z)
            {
                EndGame();
                stackTile.GetComponent<Rigidbody>().useGravity = true;
                stackTile.GetComponent<Rigidbody>().isKinematic = false;
                return;
            }

            Direction tileDirection = stackTile.GetComponent<MoveTile>().MovementDirection;
            if (tileDirection == Direction.forward || tileDirection == Direction.back)
            {
                if (stacked == null)
                    stacked = Instantiate(baseTile);
                if (stackTile.transform.position.z <= stacked.transform.position.z + errorMargin &&
                   stackTile.transform.position.z >= stacked.transform.position.z - errorMargin)
                {
                    print("entrou if error margin frente tras");
                   /* GameObject first = Instantiate(stackTile);
                    first.transform.position = new Vector3(stackTile.transform.position.x, stackTile.transform.position.y+ 0.07f, stacked.transform.position.z);
                    first.name = "Tile " + ScoreManager.Score.ToString();
                    first.GetComponent<Rigidbody>().useGravity = true;
                    first.GetComponent<MoveTile>().MovementDirection = stackTile.GetComponent<MoveTile>().MovementDirection;*/

                    CreateNextTile(stackTile);
                    entrouMargin = true;
                }
            }
            if (tileDirection == Direction.left || tileDirection == Direction.right)
            {
                if (stacked == null)
                    stacked = Instantiate(baseTile);
                print("esquerda direita");
                if (stackTile.transform.position.x <= stacked.transform.position.x + errorMargin &&
                   stackTile.transform.position.x >= stacked.transform.position.x - errorMargin)
                {
                    print("entrou if error margin esquerda direita");

                    /*GameObject first = Instantiate(stackTile);
                    first.name = "tile " + ScoreManager.Score.ToString();
                    first.GetComponent<Rigidbody>().useGravity = true;
                    first.transform.position = new Vector3(stacked.transform.position.x, stackTile.transform.position.y + 0.07f, stackTile.transform.position.z);
                    first.GetComponent<MoveTile>().MovementDirection = stackTile.GetComponent<MoveTile>().MovementDirection;*/

                    CreateNextTile(stackTile);
                    entrouMargin = true;
                }
            }

            print("pre split collider");
            if (!entrouMargin)
                SplitByCollider();
            else
                entrouMargin = false;
        }
    }

    private void PlaceBaseTile()
    {
        if (!indicador.activeInHierarchy)
        {
            return;
        }


        Vector3 baseTilePos = new Vector3(indicador.transform.position.x, indicador.transform.position.y + 0.025f, indicador.transform.position.z);
        baseTile.transform.SetPositionAndRotation(baseTilePos, indicador.transform.rotation);
        baseTile.AddComponent<OnTileCollision>();
        baseTile.GetComponent<BoxCollider>().enabled = true;
        baseTile.SetActive(true);
        baseCollider = baseTile.GetComponent<BoxCollider>();

        Vector3 posNewTile = new Vector3(indicador.transform.position.x - 0.15f, baseTile.transform.position.y + 0.07f, indicador.transform.position.z);
        stackTile.transform.SetPositionAndRotation(posNewTile, indicador.transform.rotation);
        stackTile.GetComponent<MoveTile>().MovementDirection = Direction.right;
        stackTile.GetComponent<MoveTile>().enabled = true;
        stackTile.GetComponent<BoxCollider>().enabled = true;
        stackTile.SetActive(true);
        tileCollider = stackTile.GetComponent<BoxCollider>();
    }

    private void SplitByCollider()
    {
        print("in split collider");
        Direction tileDirection = stackTile.GetComponent<MoveTile>().MovementDirection;

        GameObject first = Instantiate(stackTile);
        first.name = "first " + ScoreManager.Score.ToString();
        GameObject second = Instantiate(stackTile);
        second.name = "second";

        print("pos first second");

        if (tileDirection == Direction.left || tileDirection == Direction.right)
        {
            print("split colider left right");
            if (tileCollider.bounds.max.x > baseCollider.bounds.min.x &&
               tileCollider.bounds.max.x < baseCollider.bounds.max.x)
            {
                print("split colider 1 if");
                float firstSize = Mathf.Abs(baseCollider.bounds.min.x - tileCollider.bounds.max.x);
                float secondSize = Mathf.Abs(baseCollider.bounds.min.x - tileCollider.bounds.min.x);

                first.transform.localScale = new Vector3(firstSize, first.transform.localScale.y, first.transform.localScale.z);
                first.transform.position = new Vector3(baseCollider.bounds.min.x + (first.transform.localScale.x / 2), first.transform.position.y, first.transform.position.z);

                second.transform.localScale = new Vector3(secondSize, second.transform.localScale.y, second.transform.localScale.z);
                second.transform.position = new Vector3(baseCollider.bounds.min.x - (second.transform.localScale.x / 2), second.transform.position.y, second.transform.position.z);

            }

            if (tileCollider.bounds.min.x < baseCollider.bounds.max.x &&
              tileCollider.bounds.max.x > baseCollider.bounds.max.x)
            {
                print("split colider 2 if");
                float firstSize = Mathf.Abs(baseCollider.bounds.max.x - tileCollider.bounds.min.x);
                float secondSize = Mathf.Abs(baseCollider.bounds.max.x - tileCollider.bounds.max.x);

                first.transform.localScale = new Vector3(firstSize, first.transform.localScale.y, first.transform.localScale.z);
                first.transform.position = new Vector3(baseCollider.bounds.max.x - (first.transform.localScale.x / 2), first.transform.position.y, first.transform.position.z);

                second.transform.localScale = new Vector3(secondSize, second.transform.localScale.y, second.transform.localScale.z);
                second.transform.position = new Vector3(baseCollider.bounds.max.x + (second.transform.localScale.x / 2), second.transform.position.y, second.transform.position.z);
            }
        }

        if (tileDirection == Direction.forward || tileDirection == Direction.back)
        {
            print("split colider forward back");
            if (tileCollider.bounds.max.z > baseCollider.bounds.min.z &&
               tileCollider.bounds.max.z < baseCollider.bounds.max.z)
            {
                print("split colider forward back 1");
                float firstSize = Mathf.Abs(baseCollider.bounds.min.z - tileCollider.bounds.max.z);
                float secondSize = Mathf.Abs(baseCollider.bounds.min.z - tileCollider.bounds.min.z);

                first.transform.localScale = new Vector3(first.transform.localScale.x, first.transform.localScale.y, firstSize);
                first.transform.position = new Vector3(first.transform.position.x, first.transform.position.y, baseCollider.bounds.min.z + (first.transform.localScale.z / 2));

                second.transform.localScale = new Vector3(second.transform.localScale.x, second.transform.localScale.y, secondSize);
                second.transform.position = new Vector3(second.transform.position.x, second.transform.position.y, baseCollider.bounds.min.z - (second.transform.localScale.z / 2));

            }

            if (tileCollider.bounds.min.z < baseCollider.bounds.max.z &&
              tileCollider.bounds.max.z > baseCollider.bounds.max.z)
            {

                print("split colider forward back 2");
                float firstSize = Mathf.Abs(baseCollider.bounds.max.z - tileCollider.bounds.min.z);
                float secondSize = Mathf.Abs(baseCollider.bounds.max.z - tileCollider.bounds.max.z);

                first.transform.localScale = new Vector3(first.transform.localScale.x, first.transform.localScale.y, firstSize);
                first.transform.position = new Vector3(first.transform.position.x, first.transform.position.y, baseCollider.bounds.max.z - (first.transform.localScale.z / 2));

                second.transform.localScale = new Vector3(second.transform.localScale.x, second.transform.localScale.y, secondSize);
                second.transform.position = new Vector3(second.transform.position.x, second.transform.position.y, baseCollider.bounds.max.z + (second.transform.localScale.z / 2));
            }
        }

        print("first second use gravity");
        first.GetComponent<Rigidbody>().useGravity = true;
        second.GetComponent<Rigidbody>().useGravity = true;
        second.GetComponent<Rigidbody>().isKinematic = false;

        first.GetComponent<MoveTile>().MovementDirection = stackTile.GetComponent<MoveTile>().MovementDirection;

        print("stack pre destroy");
        stackTile.GetComponent<Renderer>().enabled = false;
        stackTile.GetComponent<BoxCollider>().enabled = false;
        Destroy(stackTile);

        CreateNextTile(first);
    }

    private void CreateNextTile(GameObject firstTile)
    {
        print("create next tile");
        if (firstTile.transform.localScale.x <= 0 || firstTile.transform.localScale.z <= 0)
        {
            print("if endgame");
            EndGame();
            return;
        }

        print("pos if endgame");
        stacked = firstTile;
        stacked.GetComponent<MoveTile>().MovementDirection = firstTile.GetComponent<MoveTile>().MovementDirection;
        stackTile = Instantiate(firstTile);
        stacked.AddComponent<OnTileCollision>();
        stackTile.GetComponent<Rigidbody>().useGravity = false;

        tileCollider = stackTile.GetComponent<BoxCollider>();
        baseCollider = stacked.GetComponent<BoxCollider>();


        if (stacked.GetComponent<MoveTile>().MovementDirection == Direction.left || stacked.GetComponent<MoveTile>().MovementDirection == Direction.right)
        {
            print("back");
            stackTile.transform.position = new Vector3(stacked.transform.position.x, stacked.transform.position.y + 0.07f, stacked.transform.position.z + 0.15f);
            stackTile.GetComponent<MoveTile>().MovementDirection = Direction.back;
            stackTile.GetComponent<Renderer>().material.color = Color.blue;

        }
        else
        {
            print("right");
            stackTile.transform.position = new Vector3(stacked.transform.position.x - 0.15f, stacked.transform.position.y + 0.07f, stacked.transform.position.z);
            stackTile.GetComponent<MoveTile>().MovementDirection = Direction.right;
            stackTile.GetComponent<Renderer>().material.color = Color.red;

        }

        print("movetileTrue");
        stackTile.GetComponent<MoveTile>().enabled = true;
        ScoreManager.Score++;
    }


    private void EndGame()
    {
        print("endgame");
    }
}

