﻿using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections;

/// <summary>
/// Player manager.
/// Handles fire Input and Beams.
/// </summary>
public class PlayerManager : Photon.PunBehaviour
{

    #region Public Variables

    [Tooltip("The Beams GameObject to control")]
    public GameObject Beams;

    [Tooltip("The current Health of our player")]
    public float Health = 1f;

    #endregion

    #region Private Variables

    //True, when the user is firing
    bool IsFiring;

    #endregion

    #region MonoBehaviour CallBacks

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {
        if (Beams == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
        }
        else
        {
            Beams.SetActive(false);
        }

    }

    void Update()
    {
        ProcessInputs();

        if(Health <= 0f)
        {
            GameManager.Instance.LeaveRoom();
        }

        // trigger Beams active state
        if (Beams != null && IsFiring != Beams.GetActive())
        {
            Beams.SetActive(IsFiring);
        }
    }

    #endregion

    #region Custom

    /// <summary>
    /// Processes the inputs. Maintain a flag representing when the user is pressing Fire.
    /// </summary>
    void ProcessInputs()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            if (!IsFiring)
            {
                IsFiring = true;
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (IsFiring)
            {
                IsFiring = false;
            }
        }
    }
    #endregion

    /// <summary>
    /// MonoBehaviour method called when the Collider 'other' enters the trigger.
    /// Affect Health of the Player if the collider is a beam
    /// Note: when jumping and firing at the same, you'll find that the player's own beam intersects with itself
    /// One could move the collider further away to prevent this or check if the beam belongs to the player.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {


        if (!photonView.isMine)
        {
            return;
        }


        // We are only interested in Beamers
        // we should be using tags but for the sake of distribution, let's simply check by name.
        if (!other.name.Contains("Beam"))
        {
            return;
        }


        Health -= 0.1f;
    }


    /// <summary>
    /// We're going to affect health while the beams are touching the player
    /// </summary>
    /// <param name="other">Other.</param>
    void OnTriggerStay(Collider other)
    {


        // we dont' do anything if we are not the local player.
        if (!photonView.isMine)
        {
            return;
        }


        // We are only interested in Beamers
        // we should be using tags but for the sake of distribution, let's simply check by name.
        if (!other.name.Contains("Beam"))
        {
            return;
        }


        // we slowly affect health when beam is constantly hitting us, so player has to move to prevent death.
        Health -= 0.1f * Time.deltaTime;
    }
}