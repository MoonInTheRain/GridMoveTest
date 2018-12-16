using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GridMoveManager : MonoBehaviour {
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private GameObject _gridPanel;

    [SerializeField]
    private GameObject _viewPanel;

    [SerializeField]
    private GameObject _baseGridObject;

    [SerializeField]
    private GridMoveObject _baseViewObject;

    public List<GridMoveObject> GridObjectList = new List<GridMoveObject>();

    public void AddObject()
    {
        var newGridObject = Instantiate(_baseGridObject, _gridPanel.transform);

        var newMoveObject = Instantiate(_baseViewObject, _viewPanel.transform);
        newMoveObject.transform.localScale = Vector3.zero;
        newMoveObject.TargetGrid = newGridObject;

        var text = newMoveObject.GetComponentInChildren<Text>();
        text.text = string.Format("{0}", _viewPanel.transform.GetComponentsInChildren<GridMoveObject>().Length);

        GridObjectList.Add(newMoveObject);
    }

    public void OutputLogList()
    {
        Debug.Log(string.Join(",", GridObjectList.OrderBy(x => x.Index).Select(x => x.GetComponentInChildren<Text>().text).ToArray()));
    }
}
