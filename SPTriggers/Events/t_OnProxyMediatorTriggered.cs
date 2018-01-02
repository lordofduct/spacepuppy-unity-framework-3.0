using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    /// <summary>
    /// Trigger that responds to the ProxyMediator.OnTriggered event.
    /// 
    /// Associate a ProxyMediator to facilitate communication between transient assets like prefab instances.
    /// </summary>
    public class t_OnProxyMediatorTriggered : TriggerComponent
    {

        [SerializeField]
        private ProxyMediator _mediator;

        #region CONSTRUCTOR

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_mediator != null)
            {
                _mediator.OnTriggered -= this.OnMediatorTriggered;
                _mediator.OnTriggered += this.OnMediatorTriggered;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (!object.ReferenceEquals(_mediator, null))
            {
                _mediator.OnTriggered -= this.OnMediatorTriggered;
            }
        }

        #endregion

        #region Properties

        public ProxyMediator Mediator
        {
            get { return _mediator; }
            set
            {
                if (_mediator == value) return;

                if (Application.isPlaying && this.enabled)
                {
                    if (!object.ReferenceEquals(_mediator, null)) _mediator.OnTriggered -= this.OnMediatorTriggered;
                    _mediator = value;
                    if (_mediator != null)
                    {
                        _mediator.OnTriggered -= this.OnMediatorTriggered;
                        _mediator.OnTriggered += this.OnMediatorTriggered;
                    }
                }
                else
                {
                    _mediator = value;
                }
            }
        }

        #endregion

        #region Methods

        private System.EventHandler _onMediatorTriggered;
        private System.EventHandler OnMediatorTriggered
        {
            get
            {
                if (_onMediatorTriggered == null) _onMediatorTriggered = this.OnMediatorTriggered_Imp;
                return _onMediatorTriggered;
            }
        }
        private void OnMediatorTriggered_Imp(object sender, System.EventArgs e)
        {
            this.ActivateTrigger(null);
        }

        #endregion

    }

}
