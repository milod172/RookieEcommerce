import { Fragment } from 'react';
import styles from './CategoryRow.module.css';
import { Link } from 'react-router-dom';
import { useSubCategories } from '../../../hooks/categories/useSubCategory';

/*
node: object hiện tại
depth: độ sâu trong cây (0 với root)
expandedIds: array chứa id của các node đang expanded
onToggle: function toggle expand/collapse, nhận id node
isLast: boolean, node cuối cùng trong số anh em (dùng để vẽ đường tree)
ancestorIsLast: array boolean đánh dấu từng tổ tiên có phải là node cuối cùng không (dùng để vẽ đường tree)
*/

const CategoryRow = ({ node, depth = 0, expandedIds, onToggle, isLast, ancestorIsLast = [] }) => {
    const isExpanded = expandedIds.includes(node.id);

    // Dùng hasChildren từ BE thay vì check subCategories.length
    const { subCategories, isLoading } = useSubCategories(
        isExpanded && node.has_children ? node.id : null
    );

    return (
        <Fragment>
            <tr className={depth > 0 ? styles.subRow : styles.parentRow}>
                {/* Expand button col 1 — chỉ depth=0 */}
                <td className="text-center">
                    {depth === 0 && (
                        node.has_children ? (
                            <button className={styles.expandBtn} onClick={() => onToggle(node.id)}>
                                {isLoading
                                    ? <span className="spinner-border spinner-border-sm" style={{ width: '12px', height: '12px' }} />
                                    : <i className={`bi ${isExpanded ? 'bi-chevron-down' : 'bi-chevron-right'}`}></i>
                                }
                            </button>
                        ) : (
                            <span className={styles.dot}></span>
                        )
                    )}
                </td>

                {/* ID cell — tree lines + expand cho sub rows */}
                <td>
                    <div className="d-flex align-items-center">
                        {ancestorIsLast.map((ancIsLast, i) => (
                            <span
                                key={i}
                                className={styles.treeLineV}
                                style={{ borderColor: ancIsLast ? 'transparent' : undefined }}
                            />
                        ))}

                        {depth > 0 && (
                            <span className={`${styles.treeBranch} ${isLast ? styles.treeBranchLast : ''}`}>
                                {node.has_children ? (
                                    <button className={styles.expandBtn} onClick={() => onToggle(node.id)}>
                                        {isLoading
                                            ? <span className="spinner-border spinner-border-sm" style={{ width: '10px', height: '10px' }} />
                                            : <i className={`bi ${isExpanded ? 'bi-chevron-down' : 'bi-chevron-right'}`} style={{ fontSize: '10px' }}></i>
                                        }
                                    </button>
                                ) : (
                                    <span className={styles.dot}></span>
                                )}
                            </span>
                        )}

                        <span className="text-muted" style={{ fontSize: '11px' }}>{node.id.slice(0, 8)}</span>
                    </div>
                </td>

                <td>
                    <div className="fw-semibold" style={{ fontSize: depth > 0 ? '13px' : '14px' }}>
                        {node.category_name}
                    </div>
                    <small className="text-muted">{node.description}</small>
                </td>

                {/* Badge: chỉ hiện khi đã fetch xong và có data */}
                <td className="text-center">
                    {node.has_children
                        ? <span className={styles.countBadge}>{node.sub_count}</span>
                        : <span className="text-muted">—</span>}
                </td>

                <td className="text-center">
                    <span className={`${styles.status} ${node.is_deleted ? styles.inactive : styles.active}`}>
                        {node.is_deleted ? "Inactive" : "Active"}
                    </span>
                </td>

                <td className="text-end">
                    <Link to={`/categories/${node.id}`} className={`btn btn-light btn-sm ${styles.actionBtn}`}>
                        <i className="bi bi-eye"></i>
                    </Link>
                    <button className={`btn btn-light btn-sm ${styles.actionBtn}`} title="Add Sub">
                        <i className="bi bi-plus-lg"></i>
                    </button>
                </td>
            </tr>

            {/* Render sub categories sau khi fetch */}
            {isExpanded && subCategories.map((sub, idx) => (
                <CategoryRow
                    key={sub.id}
                    node={sub}
                    depth={depth + 1}
                    expandedIds={expandedIds}
                    onToggle={onToggle}
                    isLast={idx === subCategories.length - 1}
                    ancestorIsLast={[...ancestorIsLast, isLast]}
                />
            ))}
        </Fragment>
    );
};
export default CategoryRow;