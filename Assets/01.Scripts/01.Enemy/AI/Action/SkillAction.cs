public class SkillAction : AIAction
{
    public override void TakeAcion()
    {
        enemyBrain.GetComponent<AIAttack>().Skill(enemyBrain.playerTrm.position);
    }
}