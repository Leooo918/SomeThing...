public class SkillAction : AIAction
{
    public override void TakeAcion()
    {
        _enemyBrain.GetComponent<AIAttack>().Skill(_enemyBrain.playerTrm.position);
    }
}