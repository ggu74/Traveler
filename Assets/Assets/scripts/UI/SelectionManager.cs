using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{

    public Camera Cam;
    public LayerMask PlayerUnitsLayerMask;
    public LayerMask TerrainLayerMask;
    public LayerMask EnemyUnitsLayerMask;
    public LayerMask EnemyBuildingLayerMask;
    public LayerMask PlayerBuildingLayerMask;
    public GameObject SelectionCubePrefab;
    public GameObject TaskWindow;
    public GameObject TemplateTaskBtn;
    public DictionaryEditor dictionaryEditor;

    private float taskBtnWidth;
    private List<Unit> SelectedUnits;
    private Vector3 SelectionCubeHeight;
    private bool isDragging;
    private RaycastHit startDragRayHit;
    private RaycastHit endDragRayHit;
    private GameObject SelectionCube;
    public float MinDragDistance;

    public bool IsDragging
    {
        get
        {
            return isDragging;
        }

        set
        {
            isDragging = value;
        }
    }


    // Use this for initialization
    void Start()
    {
        SelectedUnits = new List<Unit>();
        SelectionCubeHeight = SelectionCubePrefab.transform.localScale;
        RectTransform transformBtn = TemplateTaskBtn.GetComponent<RectTransform>();
        taskBtnWidth = transformBtn.rect.width;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            OnRightClick();
        }
        // while in drag
        if (IsDragging)
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out endDragRayHit, Mathf.Infinity, TerrainLayerMask))
                {
                    onDragEnd();
                }
                else
                {
                    if (!SelectionCube && (endDragRayHit.point - startDragRayHit.point).magnitude > MinDragDistance)
                    {
                        SelectionCube = GameObject.Instantiate<GameObject>(SelectionCubePrefab);
                        SelectionCube.transform.position = startDragRayHit.point;
                    }
                    if (SelectionCube)
                    {
                        SelectionCube.transform.localScale = SelectionCubeHeight + endDragRayHit.point - startDragRayHit.point;
                        SelectionCube.transform.position = startDragRayHit.point + SelectionCube.transform.localScale / 2;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                onDragEnd();
            }
        }
        else if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out startDragRayHit, Mathf.Infinity, TerrainLayerMask))
            {
                IsDragging = true;
            }
        }
    }

    void onDragEnd()
    {
        IsDragging = false;
        SelectedUnits.Clear();
        while (TaskWindow.transform.childCount > 0)
        {
            Transform child = TaskWindow.transform.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
        if (SelectionCube)
        {
            Vector3 SelectionCubeScale = new Vector3(Mathf.Abs(SelectionCube.transform.localScale.x), SelectionCube.transform.localScale.y, Mathf.Abs(SelectionCube.transform.localScale.z));
            Collider[] SelectedUnitsCollider = Physics.OverlapBox(SelectionCube.transform.position, SelectionCubeScale / 2, SelectionCube.transform.rotation, PlayerUnitsLayerMask);
            for (int i = SelectedUnitsCollider.Length - 1; i > 0; i--)
            {
                Unit unit = SelectedUnitsCollider[i].GetComponentInParent<Unit>();
                if (SelectedUnits.IndexOf(unit) == -1)
                {
                    SelectedUnits.Add(unit);
                }
            }
            GameObject.Destroy(SelectionCube);
            SelectionCube = null;
        }
        else
        {
            GameObject hittedObj = GameObjectUnderMouse();
            Unit unit = hittedObj.GetComponentInParent<Unit>();
            if (unit != null)
            {
                SelectedUnits.Add(unit);
            }
        }
        if (SelectedUnits.Count > 0)
        {
            UpdateTasksUi(SelectedUnits[0].AvailableTasks);
        }
    }

    private void UpdateTasksUi(Task[] tasks)
    {
        int tasksWithSpriteCount = 0;
        for (int j = 0; j < tasks.Length; j++)
        {
            System.Type taskType = tasks[j].GetType();
            if (dictionaryEditor.SpritByTask.ContainsKey(taskType) && dictionaryEditor.SpritByTask[taskType] != null)
            {
                GameObject taskBtn = Instantiate<GameObject>(TemplateTaskBtn);
                taskBtn.transform.SetParent(TaskWindow.transform, false);
                taskBtn.GetComponent<RectTransform>().position += Vector3.right * (taskBtnWidth + TemplateTaskBtn.transform.position.x) * tasksWithSpriteCount;
                taskBtn.GetComponent<Image>().sprite = dictionaryEditor.SpritByTask[taskType];
                tasksWithSpriteCount++;
            }
        }
    }

    private int layerMaskToLayer(LayerMask layerMask)
    {
        return (int)Mathf.Log(layerMask.value, 2);//reverse operation of layerMask 
    }


    private void OnRightClick()
    {
        GameObject hittedObj = GameObjectUnderMouse();
        int hittedObjLayer = hittedObj.layer;
        if (hittedObjLayer == layerMaskToLayer(TerrainLayerMask))
        {
            ClearDeadUnits();
            SortByDistance(endDragRayHit.point);
            for (int i = 0; i < SelectedUnits.Count; i++)
            {
                if (SelectedUnits[i].IsTaskAvailable<GoToTask>())
                {
                    GoToTask goToTask = SelectedUnits[i].ChangeTask<GoToTask>();
                    goToTask.StartTask(endDragRayHit.point);
                }
            }
        }
        else if (hittedObjLayer == layerMaskToLayer(EnemyUnitsLayerMask) || hittedObjLayer == layerMaskToLayer(EnemyBuildingLayerMask))
        {
            ClearDeadUnits();
            SortByDistance(hittedObj.transform.position);
            for (int i = 0; i < SelectedUnits.Count; i++)
            {
                if (SelectedUnits[i].IsTaskAvailable<AttackTask>())
                {
                    AttackTask attackTask = SelectedUnits[i].ChangeTask<AttackTask>();
                    attackTask.StartTask(hittedObj.transform);
                }
            }
        }
    }

    private void ClearDeadUnits()
    {
        for (int i = SelectedUnits.Count-1; i>=0 ; i--)
        {
            if (SelectedUnits[i]==null)
            {
                SelectedUnits.RemoveAt(i);
            }
        }
    }

    private GameObject GameObjectUnderMouse()
    {
        Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out endDragRayHit);
        GameObject hittedObj = endDragRayHit.collider.gameObject;
        return hittedObj;
    }

    private void SortByDistance(Vector3 targetDestination)
    {
        SelectedUnits.Sort(delegate (Unit unitA, Unit unitB)
        {
            float distanceToDestinationA = (unitA.transform.position - targetDestination).magnitude;
            float distanceToDestinationB = (unitB.transform.position - targetDestination).magnitude;
            return distanceToDestinationA.CompareTo(distanceToDestinationB);
        });
    }
}
