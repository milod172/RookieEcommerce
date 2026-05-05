import { Fragment } from 'react';
import styles from './CategoryRow.module.css';


const CategoryRow = ({ node, depth = 0, expandedIds, onToggle, isLast, ancestorIsLast = [], onEdit }) => {
    const isExpanded = expandedIds.includes(node.id);

    const children = node.sub_categories ?? [];
    const hasChildren = children.length > 0;

    return (
        <Fragment>
            <tr className={depth > 0 ? styles.subRow : styles.parentRow}>
                {/* Expand button col 1 — chỉ depth=0 */}
                <td className="text-center">
                    {depth === 0 && (
                        hasChildren ? (
                            <button className={styles.expandBtn} onClick={() => onToggle(node.id)}>
                                <i className={`bi ${isExpanded ? 'bi-chevron-down' : 'bi-chevron-right'}`}></i>
                            </button>
                        ) : (
                            <span className={styles.dot}></span>
                        )
                    )}
                </td>

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
                                        <i className={`bi ${isExpanded ? 'bi-chevron-down' : 'bi-chevron-right'}`} style={{ fontSize: '10px' }}></i>
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


                <td className="text-center">
                    {hasChildren
                        ? <span className={styles.countBadge}>{children.length}</span>
                        : <span className="text-muted">—</span>}
                </td>

                <td className="text-center">
                    <span className={`${styles.status} ${node.is_deleted ? styles.inactive : styles.active}`}>
                        {node.is_deleted ? "Inactive" : "Active"}
                    </span>
                </td>

                <td className="text-end">
                    <button
                        className={`btn btn-light btn-sm ${styles.actionBtn}`}
                        onClick={() => onEdit(node)}
                    >
                        <i className="bi bi-eye"></i>
                    </button>
                </td>
            </tr>

            {/* Render sub categories sau khi fetch */}
            {isExpanded && children?.map((sub, idx) => (
                <CategoryRow
                    key={sub.id}
                    node={sub}
                    depth={depth + 1}
                    expandedIds={expandedIds}
                    onToggle={onToggle}
                    isLast={idx === children.length - 1}
                    ancestorIsLast={[...ancestorIsLast, isLast]}
                    onEdit={onEdit}
                />
            ))}
        </Fragment>
    );
};
export default CategoryRow;