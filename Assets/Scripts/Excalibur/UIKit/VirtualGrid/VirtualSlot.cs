using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Excalibur
{
	public enum VirtualSlotEffectType
	{
		None,
		PointerEnterEnlarge,
		PointerEnterBreathe,
		PointerEnterSpring,
		PointerEnterShake
	}

	public enum VirtualSlotOnScrollEffectType
	{
		None,
		Fade,
		Scale
	}

	[RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    public abstract class VirtualSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField]
		private GameObject m_Select;
        private bool interactable = true;

        [SerializeField]
        private VirtualGrid m_VirtualGrid;
        internal VirtualGrid virtualGrid
        {
            get
            {
                if (m_VirtualGrid == null)
                    m_VirtualGrid = transform.parent.GetComponent<VirtualGrid>();
                return m_VirtualGrid;
            }
        }
        private Image m_Background;
        protected Image background { get { if (m_Background == null) m_Background = GetComponent<Image>(); return m_Background; } }

        public bool IsSelected 
		{
			// get 获取是否选中
			get { return virtualGrid.IsSlotSelected(this); }
			// set 设置选中效果
			set 
			{
                SetSelectedActive(value);
            }
		}

		/// <summary>
		/// 显示隐藏选中效果Image
		/// </summary>
		public void SetSelectedActive(bool active)
        {
            if (m_Select != null && m_Select.activeSelf != active)
            {
                m_Select.SetActive(active);
            }
        }

		public IItemData ItemData
		{
			get { return virtualGrid.Internal_GetItemData(m_DataIndex); }
		}

        private VirtualSlotEffectType m_PointerEnterEffectType;
		public VirtualSlotEffectType PointerEnterEffectType 
		{
			get { return m_PointerEnterEffectType; } set { m_PointerEnterEffectType = value; } 
		}

        private VirtualSlotEffectType m_OnScrllEffectType;
        public VirtualSlotEffectType OnScrllEffectType 
		{
			get { return m_OnScrllEffectType; } set { m_OnScrllEffectType = value; } 
		}

        private RectTransform m_RectTransform;
		public RectTransform Rect
		{
			get { if (m_RectTransform == null) m_RectTransform = transform as RectTransform; return m_RectTransform; }
		}

		public Vector2 AnchoredPosition { get { return Rect.anchoredPosition; } }

		public float Width { get { return virtualGrid.SlotSize[0]; } }
		public float Height { get { return virtualGrid.SlotSize[1]; } }

		private Vector3[] m_Corners;
		public Vector3[] Corners
		{
			get
			{
				if (m_Corners == null)
					m_Corners = new Vector3[4];
				Rect.GetWorldCorners(m_Corners);
				return m_Corners;
			}
		}

        private Vector3[] ViewPortWorldCorners { get { return virtualGrid.ViewPortWorldCorners; } }
		private int m_DataIndex;
		internal int DataIndex 
		{
			get { return m_DataIndex; }
			set 
			{
                m_DataIndex = value;
                if (!virtualGrid.Internal_IndexValid(m_DataIndex))
                {
                    Internal_SetActive(false);
                }
                else
                {
                    Internal_SetActive(true);
                    Reset();
                }
            }
		}

        protected virtual void Awake() 
		{
        }

		protected virtual void OnEnable() { }

		protected virtual void Start() { }

		protected virtual void Update() { }

		protected virtual void OnDisable() { }

		protected virtual void OnDestroy() { }

		public void Reset() { OnRefresh(); }

		/// <summary>
		/// 重置该slot的数据
		/// </summary>
		public void ResetDirectly() { DataIndex = m_DataIndex; }

        protected abstract void OnRefresh();

		/// <summary>
		/// 在子类必须实现的OnRefresh中获取数据
		/// 只要能够调用Reset数据就一定不为空。数据为空直接做不显示处理
		/// </summary>
        protected T GetItemData<T>() where T : IItemData
		{
			return (T)ItemData;
		}

		/// <summary>
		/// 鼠标点击事件
		/// </summary>
		internal void Internal_OnSlotClicked()
        {
            virtualGrid.OnVirtualSlotClicked(this);
            OnClickedHandler();
			if (IsSelected)
			{
                OnSelectedHandler();
            }
			else
			{
                OnCancelSelectedHandler();
            }
        }

		/// <summary>
		/// 继承类鼠标点击的事件重写此方法，不重写可以通过grid中选择事件添加事务来触发
		/// </summary>
		protected virtual void OnClickedHandler()
		{

		}

		/// <summary>
		/// 选中时触发的事件
		/// </summary>
		protected virtual void OnSelectedHandler()
		{

		}

		/// <summary>
		/// 取消选择时触发的事件
		/// </summary>
		protected virtual void OnCancelSelectedHandler()
		{

        }

        public void OnPointerClick(PointerEventData eventData)
        {
			if (!interactable)
			{
				return;
			}
			Internal_OnSlotClicked();
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
		{
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
        }

		/// <summary>
		/// 显示隐藏slot
		/// </summary>
        internal void Internal_SetActive(bool active)
		{
			if (gameObject.activeSelf != active)
			{
				gameObject.SetActive(active);
            }
            SetSelectedActive(active ? IsSelected : false);
        }

		/// <summary>
		/// 设置位置
		/// </summary>
        internal void Internal_SetPosition(Vector2 position)
		{
			Rect.anchoredPosition = position;
		}

        /// 是否在视口上面，上、左为above
        internal bool Internal_AboveViewPort()
		{
			Vector3[] corners = Corners;
			if (virtualGrid.IsVertical)
			{
				if (corners[0].y > ViewPortWorldCorners[1].y)
				{
					return true;
				}
			}
			else
			{
				if (corners[3].x < ViewPortWorldCorners[0].x)
				{
					return true;
				}
			}
            return false;
		}

        /// 是否在视口下面，下、右为below
        internal bool Internal_BelowViewPort()
        {
            Vector3[] corners = Corners;
            if (virtualGrid.IsVertical)
            {
                if (corners[1].y < ViewPortWorldCorners[0].y)
                {
                    return true;
                }
            }
            else
            {
                if (corners[0].x > ViewPortWorldCorners[3].x)
                {
                    return true;
                }
            }
            return false;
        }

		/// <summary>
		/// 部分在视口内
		/// </summary>
		/// <returns></returns>
        internal bool Internal_PartInViewPort()
        {
            Vector3[] corners = Corners;
            if (virtualGrid.IsVertical)
            {
                if ((corners[1].y >= ViewPortWorldCorners[0].y && corners[0].y < ViewPortWorldCorners[0].y)
                 || (corners[0].y <= ViewPortWorldCorners[1].y && corners[1].y > ViewPortWorldCorners[1].y))
                {
                    return true;
                }
            }
            else
            {
                if ((corners[0].x < ViewPortWorldCorners[0].x && corners[3].x >= ViewPortWorldCorners[0].x)
                 || (corners[3].x >= ViewPortWorldCorners[3].y && corners[0].x < ViewPortWorldCorners[3].x))
                {
                    return true;
                }
            }
            return false;
        }

		/// <summary>
		/// 设置中心点、锚点、尺寸
		/// </summary>
		internal void Internal_SetPivotAnchorSize()
		{
            Rect.pivot = Vector2.one * 0.5f;
            Vector2 v = new Vector2(0, 1);
            Rect.anchorMin = v;
            Rect.anchorMax = v;
        }

		/// <summary>
		/// 设置按钮是否可点击
		/// </summary>
		protected void SetInteractable(bool interactable)
		{
			this.interactable = interactable;
		}
	}
}
