using System.Collections;
using UnityEngine;

namespace InventorySystem
{
    public class GameItem : MonoBehaviour
    {
        [SerializeField]
        public ItemStack _stack;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [Header("Throw Settings")]
        [SerializeField]
        private float _colliderEnableAfter = 1f;
        [SerializeField]
        private float _throwGravity = 2f;
        [SerializeField]
        private float _minXThrowForce = 3f;
        [SerializeField]
        private float _maxXThrowForce = 5f;
        [SerializeField]
        private float _throwYForce = 5f;

        public ItemStack Stack => _stack;

        private Collider2D _collider;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _collider.enabled = false;
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            SetupGameObject();
            StartCoroutine(EnableCollider(_colliderEnableAfter));
        }


        private void OnValidate()
        {
            SetupGameObject();

        }
        private void SetupGameObject()
        {
            if (_stack.Item == null) return;
            //SetupGameSprite
            SetupSpriteRenderer();
            AdjustNumberOfItem();
            UpdateGameOjectName();
        }

        private void SetupSpriteRenderer()
        {
            _spriteRenderer.sprite = _stack.Item.InGameSprite;
        }


        private void UpdateGameOjectName()
        {
            //ItemName (5)/(ns)
            var name = _stack.Item.Name;
            var number = _stack.IsStackable ? _stack.NumberOfItems.ToString() : "ns";

            gameObject.name = $"{name} ({number})";
        }

        private void AdjustNumberOfItem()
        {
            _stack.NumberOfItems = _stack.NumberOfItems;
        }

        public ItemStack Pick()
        {
            Destroy(gameObject);
            return _stack;
        }
        public void Throw(float xDir)
        {
            _rb.gravityScale = _throwGravity;
            var throwXFroce = Random.Range(_minXThrowForce, _maxXThrowForce);
            _rb.velocity = new Vector2(Mathf.Sign(xDir) * throwXFroce, _throwYForce);
            StartCoroutine(DisableGravity(_throwYForce));
        }

        public IEnumerator DisableGravity(float atVelocity)
        {
            yield return new WaitUntil(() => _rb.velocity.y < -atVelocity);
            _rb.velocity = Vector2.zero;
            _rb.gravityScale = 0;
        }
        public IEnumerator EnableCollider(float afterTime)
        {
            yield return new WaitForSeconds(afterTime);
            _collider.enabled = true;
        }

        public void SetStack(ItemStack itemStack)
        {
            _stack = itemStack;
        }
    }

}

