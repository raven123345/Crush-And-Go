using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ingame_menu : MonoBehaviour
{
    public enum Functions
    {
        Quit, Play_Again
    };

    public Functions function;

    [SerializeField]

    Material[] onMouseOverMtl;
    [SerializeField]
    float pulsing_speed = 1f;
    [SerializeField]
    float pulsing_amount = 2f;

    Material[] initMtl;
    Material[] newmat;

    MeshRenderer meshRenderer;

    bool mouse_enter = false;
    Vector3 init_scale;
    float time_scale = 0f;
    float sin = 0f;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        initMtl = meshRenderer.materials;
        newmat = meshRenderer.materials;
        init_scale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (mouse_enter)
        {
            time_scale += Time.deltaTime * pulsing_speed;
            sin = Mathf.Abs(Mathf.Sin(time_scale));
            transform.localScale = Vector3.Lerp(init_scale, init_scale * pulsing_amount, sin);
        }
        else
        {
            if (sin > 0f)
            {
                sin -= Time.deltaTime * pulsing_speed;
                transform.localScale = Vector3.Lerp(init_scale, init_scale * pulsing_amount, sin);
            }
        }

    }

    void OnMouseEnter()
    {
        newmat = onMouseOverMtl;
        meshRenderer.materials = newmat;
        meshRenderer.sortingOrder = 50;
        mouse_enter = true;

    }

    private void OnMouseExit()
    {
        newmat = initMtl;
        meshRenderer.materials = newmat;
        mouse_enter = false;
        time_scale = 0f;
    }

    private void OnMouseDown()
    {
        switch (function)
        {
            case Functions.Quit:
                Application.Quit();
                break;
            case Functions.Play_Again:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }

}
