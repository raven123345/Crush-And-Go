using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMenu : MonoBehaviour
{
    [SerializeField]
    Transform in_frame_pos;
    [SerializeField]
    Transform out_frame_pos;
    [SerializeField]
    AnimationCurve anim_curve;
    [SerializeField]
    float anim_speed = 1f;
    bool help_on_screen = false;
    bool game_started = false;
    float time = 0f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!game_started && GameManager.instance.gameStarted)
        {
            game_started = true;
        }
        if (Input.GetButtonDown("Help") && game_started)
        {
            help_on_screen = !help_on_screen;
            GameManager.instance.gameStarted = !GameManager.instance.gameStarted;
        }

        if (help_on_screen)
        {
            if (time < 1f)
                time += Time.deltaTime * anim_speed;

            anim_curve.Evaluate(time);

            transform.position = Vector3.Lerp(out_frame_pos.position, in_frame_pos.position, anim_curve.Evaluate(time));
        }
        else
        {
            if (time > 0f)
                time -= Time.deltaTime * anim_speed;

            anim_curve.Evaluate(time);

            transform.position = Vector3.Lerp(out_frame_pos.position, in_frame_pos.position, anim_curve.Evaluate(time));
        }
    }
}
