using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UltraTweaker.Handlers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltraTweaker.Tweaks.Impl
{
    [TweakMetadata("Fall Damage", $"{UltraTweaker.GUID}.mutator_fall_damage", "Take damage when you hit the floor too hard.", $"{UltraTweaker.GUID}.mutators", 1, "Bone", true, true)]
    public class FallDamage : Tweak
    {
        public static GameObject CrunchSound;
        public static bool Immunity;

        public override void OnTweakEnabled()
        {
            base.OnTweakEnabled();

            if (CrunchSound == null)
            {
                CrunchSound = AssetHandler.Bundle.LoadAsset<GameObject>("CrunchSound.prefab");
            }
        }

        public override void OnTweakDisabled()
        {
            base.OnTweakDisabled();
        }

        private void Update()
        {
            if (NewMovement.Instance != null)
            {
                if (NewMovement.Instance.gameObject.GetComponent<CheckVelocityOnHit>() == null)
                {
                    NewMovement.Instance.gameObject.AddComponent<CheckVelocityOnHit>();
                }
            }
        }

        public class CheckVelocityOnHit : MonoBehaviour
        {
            float Stored;
            static GameObject LastCrunch;

            public void Start()
            {
                Immunity = true;
                Invoke("RemoveImmunity", 2f);
            }

            public void OnCollisionEnter(Collision col)
            {
                if (!Immunity && (col.gameObject.layer == 8 || col.gameObject.layer == 24) && (col.gameObject.CompareTag("Floor") || col.gameObject.CompareTag("Moving")))
                {
                    if (Stored <= -40 && (!NewMovement.Instance.stillHolding || NewMovement.Instance.boostCharge <= 100f))
                    {
                        float damage = (-(int)Stored);

                        LastCrunch = Instantiate(CrunchSound, NewMovement.Instance.transform.position, Quaternion.identity, transform);
                        LastCrunch.GetComponent<AudioSource>().volume = 0.2f -(Stored / 100);

                        if (NewMovement.Instance.hp != 1 && damage >= 100)
                        {
                            damage = 99;
                        }

                        NewMovement.Instance.GetHurt((int)damage, false, 0, false, true);
                        Invoke("DestroyCrunch", 2f);
                        CameraController.Instance.CameraShake(damage / 30);
                    }
                }
            }

            public void LateUpdate()
            {
                Stored = GetComponent<Rigidbody>().velocity.y;
            }

            private void DestroyCrunch()
            {
                Destroy(LastCrunch);
            }

            public void RemoveImmunity()
            {
                Immunity = false;
            }
        }
    }
}
