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
	[RequireComponent(typeof(Button))]
    [RequireComponent(typeof(RectTransform))]
    public abstract class VirtualSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField]
		private GameObject m_Select;

        public bool IsSelected 
		{ 
			// get ��ȡ�Ƿ�ѡ��
			get { return VirtualGrid.SelectedData != default(IItemData) && ReferenceEquals(ItemData, VirtualGrid.SelectedData); }
			// set ����ѡ��Ч��
			set 
			{
                SetSelectedActive(value);
            }
		}

		/// <summary>
		/// ��ʾ����ѡ��Ч��Image
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
			get { return VirtualGrid.Internal_GetItemData(m_DataIndex); }
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

		public float Width { get { return VirtualGrid.SlotSize[0]; } }
		public float Height { get { return VirtualGrid.SlotSize[1]; } }

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

        private VirtualGrid m_VirtualGrid;
		internal VirtualGrid VirtualGrid 
		{ 
			get 
			{ 
				if (m_VirtualGrid == null)
                    m_VirtualGrid = transform.parent.GetComponent<VirtualGrid>();
				return m_VirtualGrid;
			}
		}
        private Image m_Background;
		protected Image background 
		{ get { if (m_Background == null) m_Background = GetComponent<Image>(); return m_Background; } }

		private Button m_Button;
		protected Button button
		{
			get 
			{
				if (m_Button == null) 
				{
					m_Button = GetComponent<Button>();
                    m_Button.transition = Selectable.Transition.None;
                    m_Button.onClick.AddListener(Internal_OnSlotClicked);
				}
				return m_Button;
			}
		}
        private Vector3[] ViewPortWorldCorners { get { return VirtualGrid.ViewPortWorldCorners; } }
		private int m_DataIndex;
		internal int DataIndex 
		{
			get { return m_DataIndex; }
			set 
			{
                m_DataIndex = value;
                if (!VirtualGrid.Internal_IndexValid(m_DataIndex))
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
			if (m_Button == null)
            {
                m_Button = GetComponent<Button>();
                m_Button.transition = Selectable.Transition.None;
                m_Button.onClick.AddListener(Internal_OnSlotClicked);
            }
        }

		protected virtual void OnEnable() { }

		protected virtual void Start() { }

		protected virtual void Update() { }

		protected virtual void OnDisable() { }

		protected virtual void OnDestroy() { }

		public void Reset() { OnRefresh(); }

		/// <summary>
		/// ���ø�slot������
		/// </summary>
		public void ResetDirectly() { DataIndex = m_DataIndex; }

        protected abstract void OnRefresh();

		/// <summary>
		/// ���������ʵ�ֵ�OnRefresh�л�ȡ����
		/// ֻҪ�ܹ�����Reset���ݾ�һ����Ϊ�ա�����Ϊ��ֱ��������ʾ����
		/// </summary>
        protected T GetItemData<T>() where T : IItemData
		{
			return (T)ItemData;
		}

		/// <summary>
		/// ������¼�
		/// </summary>
		internal void Internal_OnSlotClicked()
        {
            VirtualGrid.OnVirtualSlotClicked(this);
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
		/// �̳�����������¼���д�˷���������д����ͨ��grid��ѡ���¼��������������
		/// </summary>
		protected virtual void OnClickedHandler()
		{

		}

		/// <summary>
		/// ѡ��ʱ�������¼�
		/// </summary>
		protected virtual void OnSelectedHandler()
		{

		}

		/// <summary>
		/// ȡ��ѡ��ʱ�������¼�
		/// </summary>
		protected virtual void OnCancelSelectedHandler()
		{

		}

        public virtual void OnPointerEnter(PointerEventData eventData)
		{
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
        }

		/// <summary>
		/// ��ʾ����slot
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
		/// ����λ��
		/// </summary>
        internal void Internal_SetPosition(Vector2 position)
		{
			Rect.anchoredPosition = position;
		}

        /// �Ƿ����ӿ����棬�ϡ���Ϊabove
        internal bool Internal_AboveViewPort()
		{
			Vector3[] corners = Corners;
			if (VirtualGrid.IsVertical)
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

        /// �Ƿ����ӿ����棬�¡���Ϊbelow
        internal bool Internal_BelowViewPort()
        {
            Vector3[] corners = Corners;
            if (VirtualGrid.IsVertical)
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
		/// �������ӿ���
		/// </summary>
		/// <returns></returns>
        internal bool Internal_PartInViewPort()
        {
            Vector3[] corners = Corners;
            if (VirtualGrid.IsVertical)
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
		/// �������ĵ㡢ê�㡢�ߴ�
		/// </summary>
		internal void Internal_SetPivotAnchorSize()
		{
            Rect.pivot = Vector2.one * 0.5f;
            Vector2 v = new Vector2(0, 1);
            Rect.anchorMin = v;
            Rect.anchorMax = v;
        }

		/// <summary>
		/// ���ð�ť�Ƿ�ɵ��
		/// </summary>
		protected void SetInteractable(bool interactable)
		{
			m_Button.interactable = interactable;
		}
    }
}
