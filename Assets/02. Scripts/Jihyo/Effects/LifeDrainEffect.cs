using UnityEngine;


public class LifeDrainEffect : MonoBehaviour
{
    public ParticleSystem ps;
    public Transform playerTransform; // 흡수할 시전자(플레이어)
    public float curveStrength = 3.0f; // 옆으로 퍼지는 정도

    private ParticleSystem.Particle[] particles;

    void Start()
    {
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    void LateUpdate()
    {
        if (playerTransform == null) return;

        int numParticlesAlive = ps.GetParticles(particles);
        Vector3 targetPos = playerTransform.position;
        Vector3 startPos = transform.position; // 타겟(몬스터) 위치

        for (int i = 0; i < numParticlesAlive; i++)
        {
            float t = 1.0f - (particles[i].remainingLifetime / particles[i].startLifetime);

            // [핵심] 입자의 고유 Seed를 이용해 각자 다른 휘어짐 값을 가짐
            uint seed = particles[i].randomSeed;
            // -1에서 1 사이의 랜덤값을 생성하여 입자마다 고유한 경로 부여
            float randX = (seed % 100 / 50f - 1f) * curveStrength;
            float randY = (seed / 100 % 100 / 50f - 1f) * curveStrength;

            // 중간 지점을 입자마다 다르게 설정 (랜덤 곡선 핵심)
            Vector3 centerPos = (startPos + targetPos) * 0.5f;
            centerPos.x += randX;
            centerPos.y += randY;

            // 베지에 곡선으로 위치 업데이트
            particles[i].position = Vector3.Lerp(Vector3.Lerp(startPos, centerPos, t), Vector3.Lerp(centerPos, targetPos, t), t);
        }

        ps.SetParticles(particles, numParticlesAlive);
    }
}
