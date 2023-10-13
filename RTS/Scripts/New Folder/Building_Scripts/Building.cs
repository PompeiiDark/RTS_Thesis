using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Building : MonoBehaviour
{
    public string buildingName = "Building";
    public int crystalCost = 100;
    public int goldCost = 200;
    public float buildTime = 10.0f;
    public GameObject buildingPrefab; // ���ǽ���������ɺ����Ϸ����

    public bool isBuilding = false;//whether it can be placed

    private int _collisionHit = 0;//Count how many objects are currently touching the placeholder
    private Renderer _placeholderChecker;

    private float buildProgress = 0.0f;

    private void FixedUpdate()
    {
        if (_collisionHit > 0)
        {
            isBuilding = false;
            _placeholderChecker.material.SetColor("_Color", Color.red);
        }
        else
        {
            isBuilding = true;
            _placeholderChecker.material.SetColor("_Color", Color.green);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Platform") _collisionHit++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Platform") _collisionHit--;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // ��ʼ���콨��
    public void StartBuilding()
    {
        if (CanAfford() && !isBuilding)
        {
            isBuilding = true;
            // �۳���Դ
            DeductResources();
        }
    }

    // �ж��Ƿ����㹻����Դ�����콨��
    private bool CanAfford()
    {
        // ������Ϸ�е���Դ����ϵͳ���ж�
        // �����������ΪResourceManager�Ĺ�����
        int currentWood = ResourceManager.instance.GetCurrentWood();
        int currentGold = ResourceManager.instance.GetCurrentGold();
        if (currentGold >= goldCost && currentWood >= crystalCost)
        {
            return true;
        } 
        return false;
    }

    // �۳���Դ
    private void DeductResources()
    {
        // ������Ϸ�е���Դ����ϵͳ���۳���Դ
        ResourceManager.instance.SpendResources(goldCost, crystalCost);
    }

    // �����������
    private void CompleteBuilding()
    {
        isBuilding = false;
        buildProgress = 0.0f;

        // ʵ��������Ԥ�Ƽ�
        Instantiate(buildingPrefab, transform.position, Quaternion.identity);
        // �������ڽ���Ľ���
        Destroy(gameObject);
    }
}
