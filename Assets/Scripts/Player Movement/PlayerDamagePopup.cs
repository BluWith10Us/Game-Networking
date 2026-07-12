using TMPro;
using UnityEngine;

public class PlayerDamagePopup : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float lifetime = 1f;

    TextMeshPro textMesh;
    Color textColor;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        textColor = textMesh.color;
    }

    public void SetDamage(int damage)
    {
        textMesh.text = damage.ToString();
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        textColor.a -= Time.deltaTime / lifetime;
        textMesh.color = textColor;

        if (textColor.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}