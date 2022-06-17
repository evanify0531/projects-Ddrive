using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform lookAt;
    public float boundX = 0.15f;
    public float boundY = 0.10f;


    private void Start()
    {
        lookAt = GameObject.Find("Player").transform;
    }

    private void LateUpdate() //LateUpdate is called after update and fixedupdate. we want to move the camera after the player movement is finished
    {
        Vector3 delta = Vector3.zero;

        //checking bounds
        //what we want to do is get the delta vector. The delta vector will characterize the movement of the camera itself, as it follows the player.
        //We don't want the camera to be focused in on the player, but rather we want the player to move around a certain boundary witout the camera moving when the player is in it.
        //Thus if the player tries to move out of the boundary, we move the camera in the same direction as the player is moving, just enough to continue the illusion that the player is in the boundary
        //The way we do this is to first check how far the character is away from the camera position (both on global scale).
        //if the absolute value of this value is greater than the bound, the player is outside of the boundary.
        //In this case, we want to move the camera, so we need to put the appropriate values inside the delta vector so the camera can follow the player.
        //How do we do this? Well, we can move the camera with our previous deltaX value that we checked. But we also want to change the camera only to the extent that the player is outside the boundary, so we subtract the boundX from that.
        //Then we set the resulting value as a component of the delta vector. Once one cycle is done, the delta vector moves the camera, and the next frame we start with a fresh delta vector being zero.


        float deltaX = lookAt.position.x - this.transform.position.x;
        if (deltaX > boundX || deltaX < -boundX)
        {
            if (this.transform.position.x < lookAt.position.x)
            {
                delta.x = deltaX - boundX;
            }

            else
            {
                delta.x = deltaX + boundX;
            }
        }


        float deltaY = lookAt.position.y - this.transform.position.y;
        if (deltaY > boundY || deltaY < -boundY)
        {
            if (this.transform.position.y < lookAt.position.y)
            {
                delta.y = deltaY - boundY;
            }

            else
            {
                delta.y = deltaY + boundY;
            }
        }

        

        this.transform.position += new Vector3(delta.x, delta.y, 0);
    }

}