import {Project} from '../components/project/project.model';

describe('Project', () => {
    it('should create project and get attributes', () => {
        let project: Project =
            new Project('id1', 'project1', 'project one', 'All', true, true);
        expect(project).toBeDefined();
        expect(project.id).toBe('id1');
        expect(project.name).toBe('project1');
        expect(project.description).toBe('project one');
        expect(project.type).toBe('All');
        expect(project.isActive).toBe(true);
        expect(project.isPublic).toBe(true);
    });
});
