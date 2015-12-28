using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VisitorScore : Thing
{
    public GameObject surface;
    public Text scoreText;
    public Text currentUserLabel;
    public GameObject owner;

    int _score = -1;
    Color _color;

    protected ObtainableItem lastItemPassedThrough;

    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            if (_score == value)
                return;

			if (_score == -1)
				_score = 1;
			else
            	_score = value;

            scoreText.text = _score.ToString();
        }
    }

    public Color DisplayColor
    {
        get
        {
            return _color;
        }
        set
        {
            if (_color == value)
                return;

            _color = value;

            surface.GetComponent<Renderer>().material.color = _color;
        }
    }

    /*
    public bool BelongsToCurrentVisitor
    {
        set
        {
            currentUserLabel.gameObject.SetActive(value);
        }
    }
    */

    public override void UpdateContributor(Thing contributor)
    {
        base.UpdateContributor(contributor);

        On(contributor, SensorType.OnSensorExit, OnExitHoop);
    }

    void OnExitHoop(object sender)
    {
        lastItemPassedThrough = sender as ObtainableItem;

		/*
        if (lastItemPassedThrough.lastOwner == null)
            return;
		*/

		/*
        var visitor = lastItemPassedThrough.lastOwner.GetComponent<Visitor>();

        if (visitor == null)
            return;

        if (visitor.avatar.face.GetComponent<Renderer>().material.color == DisplayColor)
        {
            Score += 1;
        }
        */

		Score += 1;
    }

    void Update()
    {
		/*
        if (owner == Visitor.localVisitor.gameObject)
        {
            currentUserLabel.gameObject.SetActive(true);
        }
        */	
    }

}
