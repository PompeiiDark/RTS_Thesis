using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : UnitBehavior
{
    private GameObject currentCrystal;

    public ResourceType resourceTypeToCollect;
    public float collectionRange = 2f; // �ɼ���Χ
    public float collectionRate = 3f; // ���βɼ�����ʱ��

    private float collectionTimer = 0f;// �ѹ��ɼ�ʱ��
    private int collectedResources = 0; // �Ѿ��ɼ�����Դ����

    private bool isCollecting = false;

    float distance;

    private Base nearestBaseBuilding;

    void Start()
    {
        
    }

    void Update()
    {
        Collection();
        // �ɼ���Դ�߼�
        if (isCollecting && currentCrystal != null)
        {
            // ����Ѿ��ɼ����ˣ����ػ���
            if (collectedResources >= 10)
            {
                ReturnToBase();
                //return;
            }

            distance = Vector3.Distance(transform.position, currentCrystal.transform.position);
            if (distance <= collectionRange)
            {
                collectionTimer += Time.deltaTime;
                if (collectionTimer >= collectionRate)
                {
                    if (CollectCrystal())
                    {
                        FinishCollecting();
                    }
                }
            }
            else
            {
                // �ƶ�����Դλ��
                agent.SetDestination(currentCrystal.transform.position);
                //animator.SetBool("IsCollecting", true);
            }
        }
    }
    public void Collection()
    {
        if (IsSelected())
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Crystal"))
                    {
                        // ֻ�е��Ҽ����ˮ������ʱ�Ž��вɼ�
                        currentCrystal = hit.collider.gameObject;
                        StartCollecting();
                        //agent.SetDestination(currentCrystal.transform.position);
                    }
                    else 
                    {
                        StopCollecting();
                    }
                }    
            }
        }
    }
    public bool IsSelected()
    {
        bool unitBehaviorEnabled = this.gameObject.GetComponent<UnitBehavior>().enabled;
        if (unitBehaviorEnabled && this.gameObject.GetComponent<UnitBehavior>().IsUnit()) 
        {
            return true;
        }
        return false;
    }
    private bool CollectCrystal()
    {
        if (currentCrystal == null)
        {
            return false;
        }

        Crystal crystalComponent = currentCrystal.GetComponent<Crystal>();
        if (crystalComponent != null && crystalComponent.CollectResource())
        {
            // �ɹ��ɼ�ˮ����Դ
            return true;
        }

        return false;
    }
    private void StartCollecting()
    {
        // ��ʼ�ɼ�����ʼ����ʱ���Ͳɼ�״̬
        collectionTimer = 0f;
        isCollecting = true;
    }

    private void FinishCollecting()
    {
        collectedResources += 10;
        collectionTimer = 0f;
    }

    private void StopCollecting()
    {
        // ֹͣ�ɼ�����ղɼ�״̬
        currentCrystal = null;
        isCollecting = false;
        collectionTimer = 0f;
    }
    private void ReturnToBase()
    {
        // ��������ӷ��ػ��ص��߼�
        // ���Խ��ɼ�������Դ���ݸ�����Ļ���
        // Ȼ����Դ��ӵ� ResourceManager ��

        // Ѱ������Ļ���
        FindNearestBaseBuilding();
        if (nearestBaseBuilding != null)
        {
            // �����ｫ��Դ���ݸ�����Ļ���
            nearestBaseBuilding.StoreResources(ResourceType.Crystal, collectedResources); // ���贫��10��ˮ����Դ
        }
        collectedResources = 0;
    }
    private void FindNearestBaseBuilding()
    {
        // ������ʵ��Ѱ������Ļ��ص��߼�
        // ����ʹ�� Vector3.Distance �ȷ�����������벢�ҵ�����Ļ���
    }
}
