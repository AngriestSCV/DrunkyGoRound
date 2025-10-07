
using TMPro;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace AngryLabs.Props.DrunkyGoRound
{
    public class Card : UdonSharpBehaviour
    {
        public TMP_Text TextObject;
        public SpriteRenderer SpriteRenderer;

        void Start()
        {

        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(Card))]
    public class CardEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Card card = (Card)serializedObject.targetObject;

            base.OnInspectorGUI();

            serializedObject.Update();

            // card.TextObject = (TMP_Text)EditorGUILayout.ObjectField(card.TextObject, typeof(TMP_Text), true);
            // card.SpriteRenderer = (SpriteRenderer) EditorGUILayout.ObjectField(card.SpriteRenderer, typeof(SpriteRenderer), true);

            if (card.TextObject != null)
                card.TextObject.text = EditorGUILayout.TextArea(card.TextObject.text);
            if (card.SpriteRenderer != null)
                card.SpriteRenderer.sprite = (Sprite)EditorGUILayout.ObjectField(card.SpriteRenderer.sprite, typeof(Sprite), true);

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}