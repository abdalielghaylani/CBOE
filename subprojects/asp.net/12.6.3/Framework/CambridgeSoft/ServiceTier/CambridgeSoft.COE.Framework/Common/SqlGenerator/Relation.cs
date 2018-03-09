using System;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator
{
    /// <summary>
    /// This class keeps tracks of a relation between two tables, by means of two Field objects. It's used by several other
    /// classes. 
    /// It's independent of the DataView class, although it's used by it.
    /// </summary>
    public class Relation
    {
        #region Properties
        /// <summary>
        /// Parent Field of the relation. The Name of the table that cointains this field can be extracted from the field itself
        /// </summary>
        public Field Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
            }
        }

        /// <summary>
        /// Child Field of the relation. The Name of the table that cointains this field can be extracted from the field itself
        /// </summary>
        public Field Child
        {
            get
            {
                return this.child;
            }
            set
            {
                this.child = value;
            }
        }

        /// <summary>
        /// Determines whether it is a inner or outer join.
        /// </summary>
        public bool InnerJoin
        {
            get
            {
                return this.innerJoin;
            }
            set
            {
                this.innerJoin = value;
            }
        }

        public bool LeftJoin
        {
            get { return leftJoin; }
            set { leftJoin = value; }
        }
        #endregion

        #region Variables
        /// <summary>
        /// Parent Field of the relation. The Name of the table that cointains this field can be extracted from the field itself
        /// </summary>
        private Field parent;

        /// <summary>
        /// Child Field of the relation. The Name of the table that cointains this field can be extracted from the field itself
        /// </summary>
        private Field child;

        /// <summary>
        /// Determines whether it is a inner or outer join.
        /// </summary>
        private bool innerJoin;
        private bool leftJoin;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes its members to its default values.
        /// </summary>
        public Relation()
        {
            this.parent = new Field();
            this.child = new Field();
            this.innerJoin = true;
            this.leftJoin = true;
        }

        /// <summary>
        /// Make a relationship from table and field strings
        /// </summary>
        /// <param name="parentTableName"></param>
        /// <param name="parentTableAlias"></param>
        /// <param name="parentFieldName"></param>
        /// <param name="childTableName"></param>
        /// <param name="childTableAlias"></param>
        /// <param name="childFieldName"></param>
        public Relation(string parentTableName,
                        string parentTableAlias,
                        string parentFieldName,
                        string childTableName,
                        string childTableAlias,
                        string childFieldName)
        {
            this.parent = new Field();
            this.child = new Field();
            
            this.innerJoin = true;
            this.leftJoin = true;

            Table ptbl = (Table)this.parent.Table;
            ptbl.TableName = parentTableName;
            ptbl.Alias = parentTableAlias;
            this.parent.FieldName = parentFieldName;

            Table ctbl = (Table)this.child.Table;            
            ctbl.TableName = childTableName;
            ctbl.Alias = childTableAlias;
            this.child.FieldName = childFieldName;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Two relations are equals if its parents are equals and its childs are equals.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>True if they are equals.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() == typeof(Relation))
            {
                Relation otherObj = (Relation)obj;
                if (this.Parent.FieldId == otherObj.Parent.FieldId && this.Child.FieldId == otherObj.Child.FieldId)
                {
                    if (this.Parent.Table.GetAlias() == otherObj.Parent.Table.GetAlias() && this.Child.Table.GetAlias() == otherObj.Child.Table.GetAlias())
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            string uniqueString = this.Parent.Table.ToString() + "." + this.Parent.FieldName +
                                    ":" + this.Parent.FieldType + "=" + "--" +
                                    this.Child.Table.ToString() + "." + this.Child.FieldName +
                                    ":" + this.Child.FieldType + "=";
            return uniqueString.GetHashCode();
        }

        /// <summary>
        /// Determines if two relations are equals or not.
        /// </summary>
        /// <param name="left">Left member of the operation.</param>
        /// <param name="right">Right member of the operation</param>
        /// <returns>True if equals, false otherwise.</returns>
        public static bool operator ==(Relation left, Relation right)
        {
            if (ReferenceEquals(left, right))
                return true;
            else if (ReferenceEquals(right, null))
                return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Determines if two relations are differents or not.
        /// </summary>
        /// <param name="left">Left member of the operation</param>
        /// <param name="right">Rightmember of the operation</param>
        /// <returns>True if differents, false otherwise.</returns>
        public static bool operator !=(Relation left, Relation right)
        {
            if (ReferenceEquals(left, right))
                return false;
            else if (ReferenceEquals(right, null))
                return true;
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("{0}({1}) --> {2}({3})", this.parent.GetFullyQualifiedNameString(), this.parent.FieldId, this.child.GetFullyQualifiedNameString(), this.child.FieldId);
        }
        #endregion
    }
}
