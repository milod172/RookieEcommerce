import { Fragment } from 'react';
import styles from './CategoryRow.module.css';
import { Link } from 'react-router-dom';

/*
node: object hiện tại
depth: độ sâu trong cây (0 với root)
expandedIds: array chứa id của các node đang expanded
onToggle: function toggle expand/collapse, nhận id node
isLast: boolean, node cuối cùng trong số anh em (dùng để vẽ đường tree)
ancestorIsLast: array boolean đánh dấu từng tổ tiên có phải là node cuối cùng không (dùng để vẽ đường tree)
*/

const CategoryRow = ({ node, depth = 0, expandedIds, onToggle, isLast, ancestorIsLast = [] }) => {
    const hasSubs = !!(node.subCategories?.length);
    const isExpanded = expandedIds.includes(node.id);

    return (
        <Fragment>
            <tr className={depth > 0 ? styles.subRow : styles.parentRow}>
                {/* Expand button — ở cột 1 chỉ hiện với depth=0, depth>0 chuyển sang cột ID */}
                <td className="text-center">
                    {depth === 0 && (
                        hasSubs ? (
                            <button className={styles.expandBtn} onClick={() => onToggle(node.id)}>
                                <i className={`bi ${isExpanded ? 'bi-chevron-down' : 'bi-chevron-right'}`}></i>
                            </button>
                        ) : (
                            <span className={styles.dot}></span>
                        )
                    )}
                </td>

                {/* ID cell — chứa tree lines và expand button cho sub rows */}
                <td>
                    <div className="d-flex align-items-center">
                        {/* Tree lines: mỗi tầng tổ tiên vẽ một đường dọc hoặc trắng */}
                        {ancestorIsLast.map((ancIsLast, i) => (
                            <span
                                key={i}
                                className={styles.treeLineV}
                                style={{ borderColor: ancIsLast ? 'transparent' : undefined }}
                            />
                        ))}

                        {/* Branch + toggle cho sub */}
                        {depth > 0 && (
                            <span className={`${styles.treeBranch} ${isLast ? styles.treeBranchLast : ''}`}>
                                {hasSubs ? (
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
                        {node.name}
                    </div>
                    <small className="text-muted">{node.description}</small>
                </td>

                <td className="text-center">
                    {hasSubs
                        ? <span className={styles.countBadge}>{node.subCategories.length}</span>
                        : <span className="text-muted">—</span>}
                </td>

                <td className="text-center">
                    <span className={`${styles.status} ${node.isDeleted ? styles.inactive : styles.active}`}>
                        {node.is_deleted ? "Inactive" : "Active"}
                    </span>
                </td>

                <td className="text-end">
                    <Link
                        to={`/categories/${node.id}`}
                        className={`btn btn-light btn-sm ${styles.actionBtn}`}
                    >
                        <i className="bi bi-eye"></i>
                    </Link>
                    <button className={`btn btn-light btn-sm ${styles.actionBtn}`} title="Add Sub"><i className="bi bi-plus-lg"></i></button>
                </td>
            </tr>

            {/* Đệ quy render sub categories */}
            {hasSubs && isExpanded && node.subCategories.map((sub, idx) => (
                <CategoryRow
                    key={sub.id}
                    node={sub}
                    depth={depth + 1}
                    expandedIds={expandedIds}
                    onToggle={onToggle}
                    isLast={idx === node.subCategories.length - 1}
                    ancestorIsLast={[...ancestorIsLast, isLast]}
                />
            ))}
        </Fragment>
    );
};

export default CategoryRow;