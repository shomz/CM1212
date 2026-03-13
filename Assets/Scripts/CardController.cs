using UnityEngine;

public class CardController : MonoBehaviour
{
    public int Id;
    [SerializeField] private Transform frontFace;
    [SerializeField] private Animator Animator;
    [SerializeField] private Material FrontMaterial;

    public void SetId(int id)
    {
        Id = id;
        var meshRenderer = frontFace.gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = Instantiate(FrontMaterial);
        meshRenderer.material.color = Helper.GetColor(id);
    }

    public void SetFrontFace(bool on)
    {
        Animator.SetBool("Flipped", !on);
    }
}
