using UnityEngine;

public class Stepper
{
    public float Value
    {
        get { return current; }
        set { target = value; }
    }

    private float mult;
    private float target;
    private float current;

    public Stepper(float mult = 10f)
    {
        this.mult = mult;
    }

    public void Reset()
    {
        target = 0f;
        current = 0f;
    }

    public void Update(float deltaTime)
    {
        if (target > current)
        {
            current = Mathf.Min(target, current + mult * deltaTime);
        }
        else
        {
            current = Mathf.Max(target, current - mult * deltaTime);
        }
    }

    public void Update(float target, float deltaTime)
    {
        this.target = target;
        Update(deltaTime);
    }
}