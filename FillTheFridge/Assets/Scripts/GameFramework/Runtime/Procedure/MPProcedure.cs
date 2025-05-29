using System;

namespace MPStudio
{
    public class MPProcedure
    {
        /// <summary>
        /// 流程名称
        /// </summary>
        public string procedureName { get; set; }
        /// <summary>
        /// 所属流程管理
        /// </summary>
        public MPProcedureHelper procedureHelper;
        /// <summary>
        /// 流程初始化
        /// </summary>
        public virtual void OnInit() 
        { 

        }
        /// <summary>
        /// 进入流程
        /// </summary>
        /// <param name="lastProcedure">上一个离开的流程</param>
        public virtual void OnEnter(MPProcedure lastProcedure) 
        { 

        }
        /// <summary>
        /// 流程帧刷新
        /// </summary>
        public virtual void OnUpdate() 
        { 

        }
        /// <summary>
        /// 离开流程
        /// </summary>
        /// <param name="nextProcedure">下一个进入的流程</param>
        public virtual void OnLeave(MPProcedure nextProcedure) 
        { 

        }
        /// <summary>
        /// 销毁流程
        /// </summary>
        public virtual void OnDestroy() {
            
        }
        /// <summary>
        /// 切换流程
        /// </summary>
        public virtual void SwitchProcedure(MPProcedure procedure)
        {
           MPProcedureManager.Inst.SwitchProcedure(procedure);
        }

        /// <summary>
        /// 切换流程
        /// </summary>
        public virtual void SwitchProcedure(string procedureName)
        {
           MPProcedureManager.Inst.SwitchProcedure(procedureName);
        }
        /// <summary>
        /// 切换至下一流程
        /// </summary>
        public virtual void SwitchNextProcedure()
        {
            MPProcedureManager.Inst.SwitchNextProcedure();
        }
        /// <summary>
        /// 切换至上一流程
        /// </summary>
        public virtual void SwitchLastProcedure()
        {
            MPProcedureManager.Inst.SwitchLastProcedure();
        }
    }
}